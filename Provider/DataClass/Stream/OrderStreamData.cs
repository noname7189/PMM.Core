using Binance.Net.Objects.Models.Futures.Socket;
using PMM.Core.Enum;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Impl;
using System.Diagnostics.CodeAnalysis;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class OrderStreamData
    {
        [SetsRequiredMembers]
        public OrderStreamData(BinanceFuturesStreamOrderUpdateData data, DateTime createTime)
        {
            Fee = data.Fee;
            RealizedProfit = data.RealizedProfit;
            Price = data.Price;
            AveragePrice = data.AveragePrice;

            FulfilledQuantity = data.AccumulatedQuantityOfFilledTrades;
            QuantityFilled = data.QuantityOfLastFilledTrade;
            Quantity = data.Quantity;

            UpdateTime = data.UpdateTime;
            CreateTime = createTime;

            TradeId = data.TradeId;
            OrderId = data.OrderId;

            IsMaker = data.BuyerIsMaker;
            Symbol = JKorfProvider.SymbolConverter(data.Symbol);
            Side = JKorfProvider.SideConverter(data.Side);
            Status = JKorfProvider.StatusConverter(data.Status);
        }

        public required decimal Fee;
        public required decimal RealizedProfit;
        public required decimal Price;
        public required decimal AveragePrice;

        public required decimal FulfilledQuantity;
        public required decimal QuantityFilled;
        public required decimal Quantity;

        public required DateTime UpdateTime;
        public required DateTime CreateTime;

        public required long TradeId;
        public required long OrderId;

        public required bool IsMaker;
        public required Symbol Symbol;
        public required OrderPosition Side;
        public required OrderStatusType Status;
    }
}
