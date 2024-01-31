using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Utils;
using PMM.Core.Interface;

namespace PMM.Core.CoreClass
{
    public class StrategyManagerOptions(string publicKey, string secretKey, Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? onAccountUpdate, Action<DataEvent<BinanceStreamEvent>>? onListenKeyExpired, Action<BinanceFuturesAccountInfo>? onGetAccountInfo, int baseCandleCount = 900, int initCandleCount = 1500)
    {
        public readonly string PublicKey = publicKey;
        public readonly string SecretKey = secretKey;
        public readonly int BaseCandleCount = baseCandleCount;
        public readonly int InitCandleCount = initCandleCount;
        public readonly Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? OnAccountUpdate = onAccountUpdate;
        public readonly Action<DataEvent<BinanceStreamEvent>>? OnListenKeyExpired = onListenKeyExpired;
        public readonly Action<BinanceFuturesAccountInfo>? OnGetAccountInfo = onGetAccountInfo;
    }
    public class StrategyManager
    {
        #region Singleton
        private StrategyManager() { }
        private static readonly StrategyManager instance = new();
        public static void Init(StrategyManagerOptions options)
        {
            instance.Initialized = true;
            instance.PublicKey = options.PublicKey;
            instance.SecretKey = options.SecretKey;
            instance.BaseCandleCount = options.BaseCandleCount;
            instance.InitCandleCount = options.InitCandleCount;
            instance.OnAccountUpdate = options.OnAccountUpdate;
            instance.OnGetAccountInfo = options.OnGetAccountInfo ?? ((data) => { });
            instance.OnListenKeyExpired = options.OnListenKeyExpired ?? ((data) => { });
        }

        public static StrategyManager Instance { get { return instance; } }
        #endregion
        internal int BaseCandleCount { get; set; }
        internal int InitCandleCount { get; set; }

        #region Private Property
        private bool Initialized { get; set; } = false;
        private string ListenKey { get; set; } = "";
        private string PublicKey { get; set; } = "";
        private string SecretKey { get; set; } = "";
        private readonly List<IStreamCore> _streamCoreList = [];
        private event Action<DataEvent<BinanceFuturesStreamOrderUpdate>> Chain_OnOrderUpdate = null;
        private Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? OnAccountUpdate { get; set; } = null;
        private Action<BinanceFuturesAccountInfo>? OnGetAccountInfo { get; set; } = null;
        private Action<DataEvent<BinanceStreamEvent>>? OnListenKeyExpired { get; set; } = null;
        #endregion

        #region Public Method
        public S AddStreamCore<S>()
            where S : IStreamCore, new()
        {
            S adder = new();
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(adder.Symbol, adder.Interval)) throw new Exception($"Stream core with symbol: {adder.Symbol}, interval: {adder.Interval} already exists");
            }

            _streamCoreList.Add(adder);

            return adder;
        }
        public void Run(bool keepRunning = false)
        {
            Init().Wait();
            StartStream().Wait();
            if (keepRunning)
            {
                new CancellationTokenSource().Token.WaitHandle.WaitOne();
            }
        }
        #endregion

        #region Private Method
        private BaseStreamCore<X, C> GetStreamCoreFromSymbolAndInterval<X,C>(Symbol symbol, KlineInterval interval)
            where X : DbContext, new()
            where C : BaseCandle, new()
        {
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(symbol, interval)) return (BaseStreamCore<X,C>)core;
            }

            throw new Exception($"No StreamCore with symbol: {symbol}, interval: {interval}");
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

            ApiCredentials credentials = new(PublicKey, SecretKey);
            BinanceRestClient.SetDefaultOptions((opt) =>
            {
                opt.ApiCredentials = credentials;
            });
            BinanceSocketClient.SetDefaultOptions((opt) =>
            {
                opt.ApiCredentials = credentials;
            });

            using var client = new BinanceRestClient();
            await Util.HandleRequest(() => client.UsdFuturesApi.Account.GetAccountInfoAsync(), OnGetAccountInfo!);

            var listenKey = await client.UsdFuturesApi.Account.StartUserStreamAsync();
            ListenKey = listenKey.Data;
            KeepAliveScheduler.Run(ListenKey);

            BindOrderUpdateProcess();
            
            foreach (var core in _streamCoreList)
            {
                core.BindStrategy();

                core.PreStreamInit();
                core.ExecuteChain_PreStrategyInit();
                
                await Util.HandleRequest(() => client.UsdFuturesApi.ExchangeData.GetKlinesAsync(core.Symbol.ToString(), core.Interval, limit: InitCandleCount), core.OnGetBaseCandle());

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
        }
        private Action<DataEvent<BinanceFuturesStreamOrderUpdate>>? OnOrderUpdate()
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
            var socketClient = new BinanceSocketClient();

            await socketClient.UsdFuturesApi.SubscribeToUserDataUpdatesAsync(ListenKey, null, null, onAccountUpdate: OnAccountUpdate, onOrderUpdate: OnOrderUpdate(), onListenKeyExpired: OnListenKeyExpired!, null, null, null);

            foreach (var core in _streamCoreList)
            {
                var coinSubs = await socketClient.UsdFuturesApi.SubscribeToKlineUpdatesAsync(core.Symbol.ToString(), core.Interval, core.OnGetStreamData());
                if (!coinSubs.Success)
                {
                    throw new Exception($"Subscribe for \"{core.Symbol}\" is failed");
                }

                // TODO : ConnectionLost, ConnectionRestored
                coinSubs.Data.ConnectionLost += () => throw new Exception($"Subscribe for \"{core.Symbol}\" is disconnected");
            }
        }
        #endregion
    }
}
