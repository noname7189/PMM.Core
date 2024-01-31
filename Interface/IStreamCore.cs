using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using PMM.Core.EntityClass;
using PMM.Core.Enum;

namespace PMM.Core.Interface
{
    public abstract class IStreamCore
    {
        #region Property
        public abstract Symbol Symbol { get; }
        public abstract KlineInterval Interval { get; }
        public abstract List<Action<DataEvent<BinanceFuturesStreamOrderUpdate>>> OrderCallbackList { get; }
        #endregion
        #region Util
        internal abstract bool Exists(Symbol symbol, KlineInterval interval);
        internal abstract bool AddedCandleExists();
        #endregion
        public abstract IStreamCore AddStrategy<S>() where S : IStrategy, new();
        public abstract void PreStreamInit();
        public abstract void PostStreamInit();
        public abstract Action<IEnumerable<IBinanceKline>> OnGetBaseCandle();
        public abstract Action<DataEvent<IBinanceStreamKlineData>> OnGetStreamData();
        public abstract void InitStreamWithoutAdditionalCandles();
        public abstract void InitStreamWithAdditionalCandles();

        #region Interface Declaration
        internal abstract void BindStrategy();
        internal abstract void ExecuteChain_TryToMakeNewIndicator();
        internal abstract void ExecuteChain_ProcessWithSameCandle(IBinanceStreamKline klines);
        internal abstract void ExecuteChain_ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle);
        internal abstract void ExecuteChain_PreStrategyInit();
        internal abstract void ExecuteChain_InitStrategyWithoutAdditionalCandles();
        internal abstract void ExecuteChain_InitStrategyWithAdditionalCandles();
        internal abstract void ExecuteChain_PostStrategyInit();
        #endregion
    }
}
