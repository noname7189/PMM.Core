﻿using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using PMM.Core.CoreClass;
using PMM.Core.DataClass;
using PMM.Core.EntityClass;
using PMM.Core.Enum;
using PMM.Core.Provider.DataClass;
using PMM.Core.Provider.DataClass.Stream;
using PMM.Core.Provider.Interface;

namespace PMM.Core.Interface
{
    public interface IInitProcess // PreStream
    {
        /// <summary>
        /// User defined pre-initialization (e.g., Fetch indicator list)
        /// </summary>
        public void PreStrategyInit();
        public void PreStrategyInitWrapper();
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
        public void PostStrategyInit();
        public void PostStrategyInitWrapper();
    }

    public interface IOnlineProcess // IntraStream
    {
        public void TryToMakeNewIndicator();
        public void TryToMakeNewIndicatorWrapper();
        public void ProcessWithSameCandle(KlineStreamRawData klines);
        public void ProcessWithDifferentCandle(KlineStreamRawData klines, BaseCandle prevCandle);
    }

    public interface IOrderProcess
    {
        public void ProcessEnter(decimal enterPrice, BaseSignal target);
        public void ProcessTakeProfit(decimal exitPrice, DateTime exitTime);
        public void ProcessLosscut(DateTime exitTime, BaseSignal target);
        public Action<OrderStreamRecv>? ProcessOnOrderUpdate();
    }

    public interface IStrategy : IInitProcess, IOnlineProcess, IOrderProcess
    {
        public void SetStreamCore<X,C>(BaseStreamCore<X,C> core)
            where X: DbContext, new()
            where C : BaseCandle, new();
        public void SetRestClientAdapter(IRestClientAdapter adapter);
    }
}
