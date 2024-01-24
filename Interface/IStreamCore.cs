using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using PMM.Core.EntityClass;
using PMM.Core.Enum;

namespace PMM.Core.Interface
{
    public interface IStreamCore
    {
        #region Property
        public Symbol Symbol { get; set; }
        public KlineInterval Interval { get; set; }
        public List<Action<DataEvent<BinanceFuturesStreamOrderUpdate>>> OrderCallbackList { get; }
        #endregion
        #region Util
        bool Exists(Symbol symbol, KlineInterval interval);
        bool AddedCandleExists();
        #endregion

        #region Interface Declaration
        void BindStrategy();
        void AddStrategy<S>() where S : IStrategy, new();
        void PreStreamInit();
        void InitStreamWithoutAdditionalCandles();
        void InitStreamWithAdditionalCandles();
        void PostStreamInit();
        Action<IEnumerable<IBinanceKline>> OnGetBaseCandle();
        Action<DataEvent<IBinanceStreamKlineData>> OnGetStreamData();
        void ExecuteChain_TryToMakeNewIndicator();
        void ExecuteChain_ProcessWithSameCandle(IBinanceStreamKline klines);
        void ExecuteChain_ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle);
        void ExecuteChain_PreStrategyInit();
        void ExecuteChain_InitStrategyWithoutAdditionalCandles();
        void ExecuteChain_InitStrategyWithAdditionalCandles();
        void ExecuteChain_PostStrategyInit();
        #endregion
    }
}
