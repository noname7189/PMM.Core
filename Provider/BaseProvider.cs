using PMM.Core.Interface;
using Microsoft.EntityFrameworkCore;
using PMM.Core.CoreClass;
using PMM.Core.EntityClass;
using PMM.Core.Utils;
using PMM.Core.Enum;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.DataClass;
using PMM.Core.Provider.Interface;
using PMM.Core.Provider.DataClass.Stream;

namespace PMM.Core.Provider
{
    internal abstract class BaseProvider : IProvider, IRestClientAdapter, ISocketClientAdapter
    {
        #region Private Property
        protected string ListenKey { get; set; } = "";
        public string PublicKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        internal readonly int BaseCandleCount = StrategyManager.Instance.BaseCandleCount;
        internal readonly int InitCandleCount = StrategyManager.Instance.InitCandleCount;
        protected readonly List<IStreamCore> _streamCoreList = [];
        protected event Action<OrderResult> Chain_OnOrderUpdate = null;
        protected Action<AccountUpdateData>? OnAccountUpdate { get; set; } = null;
        protected Action<AccountInfo>? OnGetAccountInfo { get; set; } = null;
        protected Action<StreamEvent>? OnListenKeyExpired { get; set; } = null;

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
        public abstract Task<string?> GetListenKey();
        public abstract Task<AccountInfo?> GetAccountInfoAsync();
        public abstract Task<List<KlineData>?> GetKlinesAsync(Symbol symbol, Interval interval, int? limit);
        public abstract Task<OrderResult?> PlaceOrderAsync(Symbol symbol, OrderPosition position, decimal price, decimal quantity);
        public abstract Task<OrderResult?> CancelOrderAsync(Symbol symbol, long orderId);

        // SocketClient
        public abstract Task SubscribeToUserDataUpdatesAsync();
        public abstract Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineStreamData> onGetStreamData);
        #endregion

        #region Public Method
        public S AddStreamCore<S>() where S : IStreamCore, new()
        {
            S adder = new();
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
        internal async Task Init()
        {
            InitContext();

            CreateContext(ProviderType.Rest);
            await GetAccountInfoAsync();
            ListenKey = await GetListenKey() ?? throw new Exception("Listenkey Error");

            // TODO : Detach BinanceClient
            KeepAliveScheduler.Run(ListenKey);

            BindOrderUpdateProcess();

            foreach (var core in _streamCoreList)
            {
                core.BindStrategy();

                core.PreStreamInit();
                core.ExecuteChain_PreStrategyInit();

                await GetKlinesAsync(core.Symbol, core.Interval, InitCandleCount);

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
        protected Action<OrderResult>? OnOrderUpdate()
        {
            return Chain_OnOrderUpdate != null ?
                (OrderResult data) =>
                {
                    Chain_OnOrderUpdate.Invoke(data);
                }
            : null;
        }

        public async Task StartStream()
        {
            CreateContext(ProviderType.Socket);
            await SubscribeToUserDataUpdatesAsync();

            foreach (var core in _streamCoreList)
            {
                // TODO : OnGetStreamData - from Action<DataEvent<IBinanceStreamKlineData>> to Action<KlineData>
                await SubscribeToKlineUpdatesAsync(core.Symbol, core.Interval, core.OnGetStreamData());
            }

            DisposeContext();
            // TODO : SocketSubscribe Data Lost, Connected Handler
        }

        #endregion
    }
}
