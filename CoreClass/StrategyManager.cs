using Binance.Net.Objects.Models;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using PMM.Core.Provider;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Impl;
using PMM.Core.Provider.Binance;
using PMM.Core.Provider.Interface;
using PMM.Core.Provider.DataClass;

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
        public static StrategyManager Instance { get { return instance; } }
        #endregion

        #region Public Property
        public readonly int BaseCandleCount = 900;
        public readonly int InitCandleCount = 1500;

        #endregion

        #region Private Property
        private readonly List<BaseProvider> _providerList = [];
        #endregion

        #region Public Method
        public IProvider AddProvider(ProviderConfiguration conf)
        {
            if (conf.Exchange == Exchange.Binance)
            {
                switch (conf.LibProvider)
                {
                    case LibProvider.JKorf:
                        {
                            JKorfProvider adder = new()
                            {
                                PublicKey = conf.PublicKey,
                                SecretKey = conf.SecretKey
                            };
                            _providerList.Add(adder);
                            return adder;

                        }

                    case LibProvider.Self:
                        {
                            SelfProvider adder = new()
                            {
                                PublicKey = conf.PublicKey,
                                SecretKey = conf.SecretKey
                            };                            
                            _providerList.Add(adder);

                            return adder;
                        }
                    default:
                        break;
                }
            }
            throw new NotImplementedException();
        }

        public void Run(bool keepRunning = false)
        {
            foreach (BaseProvider provider in _providerList)
            {
                provider.Init(provider).Wait();
                provider.StartStream().Wait();
            }

            if (keepRunning)
            {
                new CancellationTokenSource().Token.WaitHandle.WaitOne();
            }
        }
        #endregion

    }
}
