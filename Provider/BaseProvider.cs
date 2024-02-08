﻿using PMM.Core.Interface;
using PMM.Core.CoreClass;
using PMM.Core.Utils;
using PMM.Core.Enum;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Interface;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;

namespace PMM.Core.Provider
{
    internal abstract class BaseProvider : IProvider, IRestClientAdapter, ISocketClientAdapter
    {
        #region Private Property
        protected string ListenKey { get; set; } = "";
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        internal readonly int BaseCandleCount = StrategyManager.Instance.BaseCandleCount;
        internal readonly int InitCandleCount = StrategyManager.Instance.InitCandleCount;
        protected readonly List<IStreamCore> _streamCoreList = [];
        private event Action<OrderStreamRecv>? Chain_OnOrderUpdate = null;
        internal Action<AccountStreamRecv>? OnAccountUpdate { get; set; }
        internal Action<AccountInfo>? OnGetAccountInfo { get; set; }
        internal Action<BaseStreamRecv>? OnListenKeyExpired { get; set; }

        protected dynamic? ClientContext { get; set; }
        #endregion

        #region Abstract
        internal abstract void CreateContext(ProviderType type);
        internal abstract void InitContext();
        internal void DisposeContext()
        {
            if (ClientContext != null && ClientContext is IDisposable)
            {
                ClientContext!.Dispose();
            }

            ClientContext = null;
        }
        // RestClient
        public abstract Task<Response<string>> GetListenKey();
        public abstract Task<Response<AccountInfo>> GetAccountInfoAsync();
        public abstract Task<Response<List<KlineData>>> GetKlinesAsync(Symbol symbol, Interval interval, int? limit);
        public abstract Task<Response<OrderResult>> PlaceOrderAsync(Symbol symbol, OrderSide position, decimal price, decimal quantity);
        public abstract Task<Response<OrderResult>> CancelOrderAsync(Symbol symbol, long orderId);

        // SocketClient
        public abstract Task SubscribeToUserDataUpdatesAsync();
        public abstract Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineStreamRawData> onGetStreamData);
        #endregion

        #region Public Method
        public S AddStreamCore<S>() where S : IStreamCore, new()
        {
            S adder = new() { };
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(adder.Symbol, adder.Interval)) throw new ArgumentException($"Stream core with symbol: {adder.Symbol}, interval: {adder.Interval} already exists");
            }

            
            _streamCoreList.Add(adder);

            return adder;
        }
        #endregion

        #region Private Method
        private void BindOrderUpdateProcess()
        {
            foreach (var core in _streamCoreList)
            {
                if (core.OrderCallbackList.Count > 0)
                {
                    foreach (var callback in core.OrderCallbackList)
                    {
                        if (Chain_OnOrderUpdate == null) Chain_OnOrderUpdate = callback;
                        else Chain_OnOrderUpdate += callback;
                    }
                }

            }
        }
        internal async Task Init(IRestClientAdapter adapter)
        {
            ListenKey = (await GetListenKey()).Data ?? throw new Exception("Listenkey Error");

            InitContext();

            CreateContext(ProviderType.Rest);
            AccountInfo? accountInfo = (await GetAccountInfoAsync()).Data;
            if (accountInfo != null) {
                OnGetAccountInfo?.Invoke(accountInfo);
            }

            KeepAliveScheduler.Run();

            BindOrderUpdateProcess();

            foreach (var core in _streamCoreList)
            {
                core.BindStrategy(adapter);

                core.PreStreamInit();
                core.ExecuteChain_PreStrategyInit();

                Response<List<KlineData>> res = await GetKlinesAsync(core.Symbol, core.Interval, InitCandleCount);
                if (res.Data != null)
                {
                    core.OnGetBaseCandle.Invoke(res.Data);
                }
                else throw new Exception("GetKlinesAsync Error");

                if (core.AddedCandleExists())
                {
                    core.InitStreamWithAdditionalCandles();
                    core.ExecuteChain_InitStrategyWithAdditionalCandles();
                }
                else
                {
                    core.InitStreamWithoutAdditionalCandles();
                    core.ExecuteChain_InitStrategyWithoutAdditionalCandles();
                }

                core.PostStreamInit();
                core.ExecuteChain_PostStrategyInit();
            }

            DisposeContext();
        }
        protected void OnOrderUpdate(OrderStreamRecv data)
        {
            if (Chain_OnOrderUpdate == null) return;

            Chain_OnOrderUpdate.Invoke(data);
        }

        protected bool CheckChainOnOrderUpdate()
        {
            if (Chain_OnOrderUpdate == null) return false;
            return true;
        }

        public async Task StartStream()
        {
            CreateContext(ProviderType.Socket);
            await SubscribeToUserDataUpdatesAsync();

            foreach (var core in _streamCoreList)
            {
                await SubscribeToKlineUpdatesAsync(core.Symbol, core.Interval, core.OnGetStreamData);
            }

            DisposeContext();
            // TODO : SocketSubscribe Data Lost, Connected Handler
        }


        #endregion
    }
}
