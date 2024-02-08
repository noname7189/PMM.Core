using PMM.Core.Provider;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Binance;
using PMM.Core.Provider.Interface;
using PMM.Core.Provider.DataClass;

namespace PMM.Core.CoreClass
{
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
                BinanceProvider adder = new()
                {
                    PublicKey = conf.PublicKey,
                    SecretKey = conf.SecretKey,
                    OnAccountUpdate = conf.OnAccountUpdate,
                    OnGetAccountInfo = conf.OnGetAccountInfo,
                    OnListenKeyExpired = conf.OnListenKeyExpired,
                };
                _providerList.Add(adder);

                return adder;
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
