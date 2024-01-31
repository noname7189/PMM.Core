using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using PMM.Core.DataClass;
using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Interface;

namespace PMM.Core.CoreClass
{
    public abstract class BaseStrategy<X, C, I, S> : IStrategy, IIndicatorRepository<X, I>, ISignalRepository<X, S>
        where X : DbContext, new()
        where C : OHLCV, new()
        where I : Indicator, new()
        where S : Signal, new()
    {
        protected List<C> Candles { get; set; }
        protected readonly List<I> Indicators = [];
        public readonly List<S> OnlineSignals = [];
        public readonly List<OnlineOrder> OnlineOrders = [];
        protected Symbol Symbol { get; set; }

        public StreamCore<X, C> StreamCore { get; private set; }
        private void SetStreamCore(StreamCore<X, C> core)
        {
            StreamCore = core;
        }
        public void SetStreamCore<X1, C1>(StreamCore<X1, C1> core)
            where X1 : DbContext, new()
            where C1 : OHLCV, new()
        {
            SetStreamCore(core);
        }

        public void PreStrategyInitWrapper()
        {
            Candles = StreamCore.Candles;
            Symbol = StreamCore.Symbol;
            PreStrategyInit();
        }

        public void PostStrategyInitWrapper()
        {
            Indicators.RemoveRange(0, Indicators.Count - StrategyManager.Instance.BaseCandleCount);
            PostStrategyInit();
        }
        public void TryToMakeNewIndicatorWrapper()
        {
            TryToMakeNewIndicator();
            Indicators.RemoveAt(0);
        }

        public abstract DbSet<I> IndicatorRepo(X db);
        public abstract DbSet<S> SignalRepo(X db);
        public abstract void InitWithAdditionalCandles();
        public abstract void InitWithoutAdditionalCandles();
        public abstract void PreStrategyInit();
        public abstract void PostStrategyInit();
        public abstract void ProcessEnter(decimal enterPrice, Signal target);
        public abstract void ProcessLosscut(DateTime exitTime, Signal target);
        public abstract Action<DataEvent<BinanceFuturesStreamOrderUpdate>>? ProcessOnOrderUpdate();
        public abstract void ProcessTakeProfit(decimal exitPrice, DateTime exitTime);
        public abstract void ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle);
        public abstract void ProcessWithSameCandle(IBinanceStreamKline klines);
        public abstract void TryToMakeNewIndicator();
        public abstract S? TryToMakeNewSignal();

        protected void FinalizeSignal(Signal signal, decimal exitPrice, DateTime exitTime)
        {
            signal.EndTime = exitTime;
            signal.ExitPrice = exitPrice;
            if (signal.EnterPrice != null)
            {
                signal.ExpectedProfit = signal.SignalType == SignalType.Long ? (exitPrice - signal.EnterPrice) : (signal.EnterPrice - exitPrice);
            }
            else signal.ExpectedProfit = null;
        }

    }
}
