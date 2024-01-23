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
    public abstract class StreamCore<X, C>(Symbol symbol, KlineInterval interval) : IStreamCore, ICandleRepository<X, C> where X : DbContext, new() where C : OHLCV, new()
    {
        #region Public Property
        public readonly Symbol Symbol = symbol;
        public readonly KlineInterval Interval = interval;
        public readonly List<C> Candles = [];
        public readonly List<C> CandleAdders = [];
        #endregion

        #region Private Property
        private readonly object LockObj = new();

        // InitEvent
        private event Action Chain_PreStrategyInit = delegate { };
        private event Action Chain_InitStrategyWithoutAdditionalCandles = delegate { };
        private event Action Chain_InitStrategyWithAdditionalCandles = delegate { };
        private event Action Chain_PostStrategyInit = delegate { };

        // OnlineEvent
        private event Action Chain_TryToMakeNewIndicator = delegate { };
        private event Action<IBinanceStreamKline> Chain_ProcessWithSameCandle = delegate { };
        private event Action<IBinanceStreamKline, OHLCV> Chain_ProcessWithDifferentCandle = delegate { };
        #endregion

        #region Internal Property
        internal List<Action<DataEvent<BinanceFuturesStreamOrderUpdate>>> OrderCallbackList = [];
        #endregion


        #region Public Util
        public bool AddedCandleExists()
        {
            // Remember the last online candle always be added into CandleAdders, so CandleAdders.Count > 0 always be true
            return CandleAdders.Count > 1;
        }
        public bool Exists(Symbol symbol, KlineInterval interval)
        {
            return Symbol == symbol && Interval == interval;
        }
        public (Symbol symbol, KlineInterval interval) GetIdentifier()
        {
            return (Symbol, Interval);
        }
        #endregion


        #region Public Method
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
            Chain_PreStrategyInit += strategy.PreInit;
            Chain_InitStrategyWithoutAdditionalCandles += strategy.InitWithoutAdditionalCandles;
            Chain_InitStrategyWithAdditionalCandles += strategy.InitWithAdditionalCandles;
            Chain_PostStrategyInit += strategy.PostInit;
        }
        private void BindOnlineProcess(IOnlineProcess strategy)
        {
            Chain_TryToMakeNewIndicator += strategy.TryToMakeNewIndicator;
            Chain_ProcessWithDifferentCandle += strategy.ProcessWithDifferentCandle;
            Chain_ProcessWithSameCandle += strategy.ProcessWithSameCandle;
        }
        #endregion

        #region Abstract Method
        public abstract DbSet<C> CandleRepo(X db);
        public abstract void PreStreamInit();
        public abstract void InitStreamWithoutAdditionalCandles();
        public abstract void InitStreamWithAdditionalCandles();
        public abstract void PostStreamInit();
        #endregion


        #region Interface Implement
        public Action<IEnumerable<IBinanceKline>> OnGetBaseCandle() => (klines) =>
        {
            if (Candles.Count == 0) throw new Exception("StreamCore has not been initialized!");
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
                }
                else
                {
                    if (targetTime == kline.OpenTime.AddHours(9)) found = true;
                }
            }

            if (found == false) throw new Exception("Candle Data Needs Up-To-Date");
        };

        public void AddStrategy(IStrategy strategy)
        {
            if (strategy.CheckIdentifier(Symbol, Interval))
            {
                BindInitProcess(strategy);
                BindOnlineProcess(strategy);
                OrderCallbackList.Add(strategy.ProcessOnOrderUpdate());
            }
            else throw new Exception("Symbol, Interval from stream differs with those from strategy");
        }
        public void ExecuteChain_PreStrategyInit()
        {
            Chain_PreStrategyInit.Invoke();
        }

        public void ExecuteChain_InitStrategyWithoutAdditionalCandles()
        {
            Chain_InitStrategyWithoutAdditionalCandles.Invoke();
        }

        public void ExecuteChain_InitStrategyWithAdditionalCandles()
        {
            Chain_InitStrategyWithAdditionalCandles.Invoke();
        }
        public void ExecuteChain_PostStrategyInit()
        {
            Chain_PostStrategyInit.Invoke();
        }


        public void ExecuteChain_TryToMakeNewIndicator()
        {
            Chain_TryToMakeNewIndicator.Invoke();
        }
        public void ExecuteChain_ProcessWithSameCandle(IBinanceStreamKline klines)
        {
            Chain_ProcessWithSameCandle.Invoke(klines);
        }

        public void ExecuteChain_ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle)
        {
            Chain_ProcessWithDifferentCandle.Invoke(klines, prevCandle);
        }


        public Action<DataEvent<IBinanceStreamKlineData>> OnGetStreamData() => (stream) =>
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

                    AddCandles([prevCandle]);
                }
            }

            if (prevCandle != null)
            {
                ExecuteChain_TryToMakeNewIndicator();
                ExecuteChain_ProcessWithDifferentCandle(klines, prevCandle);
            }
        };
        #endregion
    }
}
