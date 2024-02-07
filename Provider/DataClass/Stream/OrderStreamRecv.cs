using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;
using PMM.Core.Provider.Enum;
using System.Diagnostics.CodeAnalysis;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class OrderStreamRecv : BaseStreamRecv
    {
        [JsonProperty("n")]
        public decimal Fee;
        [JsonProperty("rp")]
        public decimal RealizedProfit;
        [JsonProperty("p")]
        public decimal Price;
        [JsonProperty("ap")]
        public decimal AveragePrice;

        [JsonProperty("z")]
        public decimal FulfilledQuantity;
        [JsonProperty("l")]
        public decimal QuantityFilled;
        [JsonProperty("q")]
        public decimal Quantity;

        [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime;

        public DateTime CreateTime;
        [JsonProperty("t")]
        public long TradeId;
        [JsonProperty("i")]
        public long OrderId;
        [JsonProperty("m")]
        public bool IsMaker;

        [JsonProperty("s")]
        public Symbol? Symbol;

        [JsonProperty("S"), JsonConverter(typeof(OrderPositionConverter))]
        public OrderSide? Side;

        [JsonProperty("o"), JsonConverter(typeof(OrderTypeConverter))]
        public OrderStatus? Status;
    }
}
