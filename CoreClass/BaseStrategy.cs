using Binance.Net.Enums;
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
    public abstract class BaseStrategy<X, I>(Symbol symbol, KlineInterval interval) : IStrategy, IIndicatorRepository<X, I>
        where X : DbContext, new()
        where I : Indicator, new()
    {
        public readonly Symbol Symbol = symbol;
        public readonly KlineInterval Interval = interval;
        public readonly List<I> Indicators = [];
        public void AddIndicators(IList<I> adder)
        {
            using X db = new();
            IndicatorRepo(db).AddRange(adder);
            db.SaveChanges();
        }
        public bool CheckIdentifier(Symbol symbol, KlineInterval interval)
        {
            return Symbol == symbol && Interval == interval;
        }

        #region IInitProcess
        public abstract void PreInit();
        public abstract void InitWithoutAdditionalCandles();
        public abstract void InitWithAdditionalCandles();
        public abstract void PostInit();
        #endregion

        #region IOnlineProcess
        public abstract void TryToMakeNewIndicator();
        public abstract void ProcessWithSameCandle(IBinanceStreamKline klines);
        public abstract void ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle);
        public abstract OnlineSignal? TryToMakeNewSignal();
        #endregion

        #region IOrderProcess
        public abstract void ProcessEnter(IBinanceStreamKline klines, OnlineSignal target);
        public abstract void ProcessTakeProfit(IBinanceStreamKline klines, OnlineSignal target);
        public abstract void ProcessLosscut(IBinanceStreamKline klines, OnlineSignal target);
        public abstract Action<DataEvent<BinanceFuturesStreamOrderUpdate>> ProcessOnOrderUpdate();
        #endregion

        #region IIndicatorRepository
        public abstract DbSet<I> IndicatorRepo(X db);
        #endregion
    }
}
