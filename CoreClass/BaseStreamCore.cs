﻿using Microsoft.EntityFrameworkCore;
using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Interface;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Interface;
using PMM.Core.Utils;

namespace PMM.Core.CoreClass
{
    public abstract class BaseStreamCore<X, C>() : IStreamCore, ICandleRepository<X, C> where X : DbContext, new() where C : BaseCandle, new()
    {
        #region Public Property
        public readonly List<C> Candles = [];
        public readonly List<C> CandleAdders = [];
        public override List<Action<OrderStreamRecv>> OrderCallbackList { get => _orderCallbackList; }
        #endregion

        #region Private Property
        private readonly List<IStrategy> _strategyList = [];
        private readonly object LockObj = new();
        private readonly List<Action<OrderStreamRecv>> _orderCallbackList = [];

        // InitEvent
        private event Action Chain_PreStrategyInit = delegate { };
        private event Action Chain_InitStrategyWithoutAdditionalCandles = delegate { };
        private event Action Chain_InitStrategyWithAdditionalCandles = delegate { };
        private event Action Chain_PostStrategyInit = delegate { };

        // OnlineEvent
        private event Action Chain_TryToMakeNewIndicator = delegate { };
        private event Action<KlineStreamRawData> Chain_ProcessWithSameCandle = delegate { };
        private event Action<KlineStreamRawData, BaseCandle> Chain_ProcessWithDifferentCandle = delegate { };
        #endregion

        #region Public Method
        internal override bool AddedCandleExists()
        {
            return CandleAdders.Count > 0;
        }
        internal override bool Exists(Symbol symbol, Interval interval)
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

        public sealed override Action<List<KlineStreamRawData>> OnGetBaseCandle() => (klines) =>
        {
            if (Candles.Count == 0) throw new ArgumentException("StreamCore has not been initialized!");
            //if (Candles.Count == 0) return;
            DateTime targetTime = Candles.Last().Time;


            bool found = false;

            foreach (var kline in klines)
            {
                // TODO : Deprecate
                DateTime startTime = kline.GetType() == typeof(KlineStreamData) ? ((KlineStreamData)kline).StartTime.AddHours(9) : Util.GetDateTimeFromMilliSeconds(kline.StartTime);

                if (found)
                {

                    CandleAdders.Add(new C() 
                    { 
                        Time = startTime,
                        Open = kline.Open,
                        High = kline.High,
                        Low = kline.Low,
                        Close = kline.Close,
                        Volume = kline.Volume
                    });

                    targetTime = Util.GetDateTimeFromMilliSeconds(kline.EndTime);
                }
                else
                {
                    if (targetTime == startTime) found = true;
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

        internal override void BindStrategy(IRestClientAdapter adapter)
        {
            foreach (var strategy in _strategyList)
            {
                strategy.SetStreamCore(this);
                BindInitProcess(strategy);
                BindOnlineProcess(strategy);
                Action<OrderStreamRecv>? callback = strategy.ProcessOnOrderUpdate();
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
        internal override void ExecuteChain_ProcessWithSameCandle(KlineStreamRawData klines)
        {
            Chain_ProcessWithSameCandle.Invoke(klines);
        }
        internal override void ExecuteChain_ProcessWithDifferentCandle(KlineStreamRawData klines, BaseCandle prevCandle)
        {
            Chain_ProcessWithDifferentCandle.Invoke(klines, prevCandle);
        }
        public sealed override Action<KlineStreamRawData> OnGetStreamData() => (stream) =>
        {
            KlineStreamRawData klines = stream;
            C? prevCandle = null;

            // TODO : Deprecate
            DateTime startTime = klines.GetType() == typeof(KlineStreamData) ? ((KlineStreamData)klines).StartTime.AddHours(9) : Util.GetDateTimeFromMilliSeconds(klines.StartTime);

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
                        Time = startTime,
                        Open = klines.Open,
                        High = klines.High,
                        Low = klines.Low,
                        Close = klines.Close,
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
