using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models;
using CryptoExchange.Net.Sockets;
using PMM.Core.Interface;
using Binance.Net.Enums;
using Microsoft.EntityFrameworkCore;
using PMM.Core.CoreClass;
using PMM.Core.EntityClass;
using PMM.Core.Utils;
using PMM.Core.Enum;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.DataClass;
using PMM.Core.Provider.Interface;

namespace PMM.Core.Provider
{
    internal abstract class BaseProvider : IProvider, IRestClientAdapter, ISocketClientAdapter
    {
        #region Private Property
        private bool Initialized { get; set; } = false;
        protected string ListenKey { get; set; } = "";
        private string PublicKey { get; set; } = "";
        internal int BaseCandleCount { get; set; }
        internal int InitCandleCount { get; set; }
        internal string SecretKey { get; set; } = "";
        protected readonly List<IStreamCore> _streamCoreList = [];
        protected event Action<DataEvent<BinanceFuturesStreamOrderUpdate>> Chain_OnOrderUpdate = null;
        protected Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? OnAccountUpdate { get; set; } = null;
        protected Action<AccountInfo>? OnGetAccountInfo { get; set; } = null;
        protected Action<DataEvent<BinanceStreamEvent>>? OnListenKeyExpired { get; set; } = null;

        protected dynamic? ClientContext { get; set; }
        #endregion

        #region Abstract
        public abstract void CreateContext(ProviderType type);
        public abstract void InitContext();
        public void DisposeContext()
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
        public abstract Task SubscribeToKlineUpdatesAsync(Symbol symbol, Interval interval, Action<KlineData> onGetStreamData);
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
        private BaseStreamCore<X, C> GetStreamCoreFromSymbolAndInterval<X, C>(Symbol symbol, KlineInterval interval)
            where X : DbContext, new()
            where C : BaseCandle, new()
        {
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(symbol, interval)) return (BaseStreamCore<X, C>)core;
            }

            throw new ArgumentException($"No StreamCore with symbol: {symbol}, interval: {interval}");
        }
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
        private async Task Init()
        {
            if (Initialized == false) throw new Exception("No Provided StrategyManagerOptions");

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

                await GetKlinesAsync(core.Symbol, core.Interval.ToInterval(), InitCandleCount);

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
        protected Action<DataEvent<BinanceFuturesStreamOrderUpdate>>? OnOrderUpdate()
        {
            return Chain_OnOrderUpdate != null ?
                (DataEvent<BinanceFuturesStreamOrderUpdate> data) =>
                {
                    Chain_OnOrderUpdate.Invoke(data);
                }
            : null;
        }

        private async Task StartStream()
        {
            CreateContext(ProviderType.Socket);
            await SubscribeToUserDataUpdatesAsync();

            foreach (var core in _streamCoreList)
            {
                // TODO : OnGetStreamData - from Action<DataEvent<IBinanceStreamKlineData>> to Action<KlineData>
                await SubscribeToKlineUpdatesAsync(core.Symbol, core.Interval.ToInterval(), core.OnGetStreamData);
            }

            DisposeContext();
            // TODO : SocketSubscribe Data Lost, Connected Handler
        }

        #endregion
    }
}
