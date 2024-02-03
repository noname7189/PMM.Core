using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Interface;

namespace PMM.Core.CoreClass
{
    public abstract class BaseStreamCore<X, C>() : IStreamCore, ICandleRepository<X, C> where X : DbContext, new() where C : BaseCandle, new()
    {
        #region Public Property
        public readonly List<C> Candles = [];
        public readonly List<C> CandleAdders = [];
        public override List<Action<DataEvent<BinanceFuturesStreamOrderUpdate>>> OrderCallbackList { get => _orderCallbackList; }

        #endregion

        #region Private Property
        private readonly List<IStrategy> _strategyList = [];
        private readonly object LockObj = new();
        private readonly List<Action<DataEvent<BinanceFuturesStreamOrderUpdate>>> _orderCallbackList = [];


        // InitEvent
        private event Action Chain_PreStrategyInit = delegate { };
        private event Action Chain_InitStrategyWithoutAdditionalCandles = delegate { };
        private event Action Chain_InitStrategyWithAdditionalCandles = delegate { };
        private event Action Chain_PostStrategyInit = delegate { };

        // OnlineEvent
        private event Action Chain_TryToMakeNewIndicator = delegate { };
        private event Action<IBinanceStreamKline> Chain_ProcessWithSameCandle = delegate { };
        private event Action<IBinanceStreamKline, BaseCandle> Chain_ProcessWithDifferentCandle = delegate { };
        #endregion

        #region Public Method
        internal override bool AddedCandleExists()
        {
            return CandleAdders.Count > 0;
        }
        internal override bool Exists(Symbol symbol, KlineInterval interval)
        {
            return Symbol == symbol && Interval == interval;
        }
        public void AddCandles(IList<C> adder)
        {
            using X db = new();
            CandleRepo(db).AddRange(adder);
            db.SaveChanges();
        }
        #endregion

        #region Private Method
        private void BindInitProcess(IInitProcess strategy)
        {
            Chain_PreStrategyInit += strategy.PreStrategyInitWrapper;
            Chain_InitStrategyWithoutAdditionalCandles += strategy.InitWithoutAdditionalCandles;
            Chain_InitStrategyWithAdditionalCandles += strategy.InitWithAdditionalCandles;
            Chain_PostStrategyInit += strategy.PostStrategyInitWrapper;
        }
        private void BindOnlineProcess(IOnlineProcess strategy)
        {
            Chain_TryToMakeNewIndicator += strategy.TryToMakeNewIndicatorWrapper;
            Chain_ProcessWithDifferentCandle += strategy.ProcessWithDifferentCandle;
            Chain_ProcessWithSameCandle += strategy.ProcessWithSameCandle;
        }
        #endregion

        #region Abstract Method
        public abstract DbSet<C> CandleRepo(X db);
        #endregion

        #region Interface Implement

        public override void InitStreamWithAdditionalCandles()
        {
            using X db = new();
            CandleRepo(db).AddRange(CandleAdders);
            db.SaveChanges();

            Candles.AddRange(CandleAdders);
        }

        public override void InitStreamWithoutAdditionalCandles()
        {

        }

        public override void PostStreamInit()
        {
            Candles.RemoveRange(0, Candles.Count - StrategyManager.Instance.BaseCandleCount);
        }

        public sealed override Action<IEnumerable<IBinanceKline>> OnGetBaseCandle() => (klines) =>
        {
            if (Candles.Count == 0) throw new ArgumentException("StreamCore has not been initialized!");
            //if (Candles.Count == 0) return;
            DateTime targetTime = Candles.Last().Time;

            bool found = false;

            foreach (var kline in klines)
            {
                if (found)
                {
                    CandleAdders.Add(new C() 
                    { 
                        Time = kline.OpenTime.AddHours(9),
                        Open = kline.OpenPrice,
                        High = kline.HighPrice,
                        Low = kline.LowPrice,
                        Close = kline.ClosePrice,
                        Volume = kline.Volume
                    });

                    targetTime = kline.CloseTime;
                }
                else
                {
                    if (targetTime == kline.OpenTime.AddHours(9)) found = true;
                }
            }

            if (found == false) throw new Exception("Candle data needs Up-To-Date");
            // The last candle is very likely not closed yet, so remove this candle.
            if (CandleAdders[^2].Time.AddMinutes(5) != targetTime)
            {
                CandleAdders.RemoveAt(CandleAdders.Count - 1);
            }
        };

        public sealed override IStreamCore AddStrategy<S>()
        {
            foreach (var strategy in _strategyList)
            {
                if (strategy.GetType() == typeof(S)) throw new ArgumentException($"{GetType()} already has {typeof(S)}");
            }

            S adder = new();
            _strategyList.Add(adder);

            return this;
        }

        internal override void BindStrategy()
        {
            foreach (var strategy in _strategyList)
            {
                strategy.SetStreamCore(this);
                BindInitProcess(strategy);
                BindOnlineProcess(strategy);
                Action<DataEvent<BinanceFuturesStreamOrderUpdate>>? callback = strategy.ProcessOnOrderUpdate();
                if (callback != null)
                {
                    OrderCallbackList.Add(callback);
                }
            }
        }

        internal override void ExecuteChain_PreStrategyInit()
        {
            Chain_PreStrategyInit.Invoke();
        }

        internal override void ExecuteChain_InitStrategyWithoutAdditionalCandles()
        {
            Chain_InitStrategyWithoutAdditionalCandles.Invoke();
        }

        internal override void ExecuteChain_InitStrategyWithAdditionalCandles()
        {
            Chain_InitStrategyWithAdditionalCandles.Invoke();
        }
        internal override void ExecuteChain_PostStrategyInit()
        {
            Chain_PostStrategyInit.Invoke();
        }


        internal override void ExecuteChain_TryToMakeNewIndicator()
        {
            Chain_TryToMakeNewIndicator.Invoke();
        }
        internal override void ExecuteChain_ProcessWithSameCandle(IBinanceStreamKline klines)
        {
            Chain_ProcessWithSameCandle.Invoke(klines);
        }
        internal override void ExecuteChain_ProcessWithDifferentCandle(IBinanceStreamKline klines, BaseCandle prevCandle)
        {
            Chain_ProcessWithDifferentCandle.Invoke(klines, prevCandle);
        }
        public sealed override Action<DataEvent<IBinanceStreamKlineData>> OnGetStreamData() => (stream) =>
        {
            IBinanceStreamKline klines = stream.Data.Data;

            C? prevCandle = null;

            lock (LockObj)
            {
                if (klines.Final == false)
                {
                    ExecuteChain_ProcessWithSameCandle(klines);
                }
                else
                {
                    prevCandle = new()
                    {
                        Time = klines.OpenTime.AddHours(9),
                        Open = klines.OpenPrice,
                        High = klines.HighPrice,
                        Low = klines.LowPrice,
                        Close = klines.ClosePrice,
                        Volume = klines.Volume
                    };

                    Task.Run(() =>
                    {
                        AddCandles([prevCandle]);
                        Candles.Add(prevCandle);
                        // Clear indicators[0] after making new
                        ExecuteChain_TryToMakeNewIndicator();
                        // Manually clear candles[0]
                        Candles.RemoveAt(0);

                        ExecuteChain_ProcessWithDifferentCandle(klines, prevCandle);
                    });
                }
            }
        };
        #endregion
    }
}
