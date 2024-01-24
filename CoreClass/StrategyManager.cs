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
    public class StrategyManagerOptions(string publicKey, string secretKey, Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? onAccountUpdate, Action<DataEvent<BinanceStreamEvent>>? onListenKeyExpired, Action<BinanceFuturesAccountInfo>? onGetAccountInfo, int baseCandleCount = 1500)
    {
        public readonly string PublicKey = publicKey;
        public readonly string SecretKey = secretKey;
        public readonly int BaseCandleCount = baseCandleCount;
        public readonly Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? OnAccountUpdate = onAccountUpdate;
        public readonly Action<DataEvent<BinanceStreamEvent>>? OnListenKeyExpired = onListenKeyExpired;
        public readonly Action<BinanceFuturesAccountInfo>? OnGetAccountInfo = onGetAccountInfo;
    }
    public class StrategyManager
    {
        #region Singleton
        private StrategyManager() { }
        private static readonly StrategyManager instance = new();
        public static StrategyManager Instance(StrategyManagerOptions options)
        {
            instance.Initialized = true;
            instance.PublicKey = options.PublicKey;
            instance.SecretKey = options.SecretKey;
            instance.BaseCandleCount = options.BaseCandleCount;
            instance.OnAccountUpdate = options.OnAccountUpdate;
            instance.OnGetAccountInfo = options.OnGetAccountInfo ?? ((data) => { });
            instance.OnListenKeyExpired = options.OnListenKeyExpired ?? ((data) => { });
            return instance;
        }
        #endregion

        #region Private Property
        private int BaseCandleCount { get; set; }
        private bool Initialized { get; set; } = false;
        private bool ChainOnOrderUpdateExists { get; set; } = false;
        private string ListenKey { get; set; } = "";
        private string PublicKey { get; set; } = "";
        private string SecretKey { get; set; } = "";
        private readonly List<IStreamCore> _streamCoreList = [];
        private event Action<DataEvent<BinanceFuturesStreamOrderUpdate>> Chain_OnOrderUpdate = delegate { };
        private Action<DataEvent<BinanceFuturesStreamAccountUpdate>>? OnAccountUpdate { get; set; } = null;
        private Action<BinanceFuturesAccountInfo>? OnGetAccountInfo { get; set; } = null;
        private Action<DataEvent<BinanceStreamEvent>>? OnListenKeyExpired { get; set; } = null;
        #endregion

        #region Public Method
        public S AddStreamCore<S>(Symbol symbol, KlineInterval interval)
            where S : IStreamCore, new()
        {
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(symbol, interval)) throw new Exception($"Stream core with symbol: {symbol}, interval: {interval} already exists");
            }

            S adder = new() { Symbol = symbol, Interval = interval};
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
        private StreamCore<X, C> GetStreamCoreFromSymbolAndInterval<X,C>(Symbol symbol, KlineInterval interval)
            where X : DbContext, new()
            where C : OHLCV, new()
        {
            foreach (var core in _streamCoreList)
            {
                if (core.Exists(symbol, interval)) return (StreamCore<X,C>)core;
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
                        if (ChainOnOrderUpdateExists == false) ChainOnOrderUpdateExists = true;
                        Chain_OnOrderUpdate += callback;
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

            BindOrderUpdateProcess();
            
            foreach (var core in _streamCoreList)
            {
                core.PreStreamInit();
                core.ExecuteChain_PreStrategyInit();
                
                await Util.HandleRequest(() => client.UsdFuturesApi.ExchangeData.GetKlinesAsync(core.Symbol.ToString(), core.Interval, limit: BaseCandleCount), core.OnGetBaseCandle());

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
            return ChainOnOrderUpdateExists == true ?
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
