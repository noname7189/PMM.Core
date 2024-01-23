using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using PMM.Core.DataClass;
using PMM.Core.EntityClass;
using PMM.Core.Enum;

namespace PMM.Core.Interface
{
    public interface IInitProcess // PreStream
    {
        /// <summary>
        /// User defined pre-initialization (e.g., Fetch indicator list)
        /// </summary>
        public void PreInit();
        /// <summary>
        /// User defined initialization without additional candles (e.g., Fetch online-order, online-signal list)
        /// </summary>
        public void InitWithoutAdditionalCandles();
        /// <summary>
        /// User defined initialization with additional candles (e.g., Flush residual signals)
        /// </summary>
        public void InitWithAdditionalCandles();
        /// <summary>
        /// User defined post-initlization (e.g., Fetch last online signal generated after initialization)
        /// </summary>
        public void PostInit();
    }

    public interface IOnlineProcess // IntraStream
    {
        public void TryToMakeNewIndicator();
        public void ProcessWithSameCandle(IBinanceStreamKline klines);
        public void ProcessWithDifferentCandle(IBinanceStreamKline klines, OHLCV prevCandle);
        public OnlineSignal? TryToMakeNewSignal();
    }

    public interface IOrderProcess
    {
        public void ProcessEnter(IBinanceStreamKline klines, OnlineSignal target);
        public void ProcessTakeProfit(IBinanceStreamKline klines, OnlineSignal target);
        public void ProcessLosscut(IBinanceStreamKline klines, OnlineSignal target);
        public Action<DataEvent<BinanceFuturesStreamOrderUpdate>> ProcessOnOrderUpdate();
    }

    public interface IStrategy : IInitProcess, IOnlineProcess, IOrderProcess
    {
        public bool CheckIdentifier(Symbol symbol, KlineInterval interval);
    }

}
