using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Impl;
using System.Diagnostics.CodeAnalysis;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class OrderStreamRecv : BaseStreamRecv
    {
        public OrderStreamRecv() { }
        [SetsRequiredMembers]
        public OrderStreamRecv(BinanceFuturesStreamOrderUpdateData data, DateTime createTime)
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

        [JsonProperty("n")]
        public required decimal Fee;
        [JsonProperty("rp")]
        public required decimal RealizedProfit;
        [JsonProperty("p")]
        public required decimal Price;
        [JsonProperty("ap")]
        public required decimal AveragePrice;

        [JsonProperty("z")]
        public required decimal FulfilledQuantity;
        [JsonProperty("l")]
        public required decimal QuantityFilled;
        [JsonProperty("q")]
        public required decimal Quantity;

        [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
        public required DateTime UpdateTime;

        public required DateTime CreateTime;
        [JsonProperty("t")]
        public required long TradeId;
        [JsonProperty("i")]
        public required long OrderId;
        [JsonProperty("m")]
        public required bool IsMaker;

        [JsonProperty("s")]
        public required Symbol Symbol;

        [JsonProperty("S"), JsonConverter(typeof(OrderPositionConverter))]
        public required OrderPosition Side;

        [JsonProperty("o"), JsonConverter(typeof(OrderTypeConverter))]
        public required OrderStatusType Status;
    }
}
