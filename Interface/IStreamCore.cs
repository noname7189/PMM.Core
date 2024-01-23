using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using PMM.Core.EntityClass;
using PMM.Core.Enum;

namespace PMM.Core.Interface
{
    internal interface IStreamCore
    {
        #region Util
        (Symbol symbol, KlineInterval interval) GetIdentifier();
        bool Exists(Symbol symbol, KlineInterval interval);
        bool AddedCandleExists();
        #endregion

        #region Interface Declaration
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
