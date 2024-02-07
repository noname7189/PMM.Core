using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class OrderResult
    {
        [JsonProperty("price")]
        public decimal Price;

        [JsonProperty("avgPrice")]
        public decimal AveragePrice;

        [JsonProperty("executedQty")]
        public decimal QuantityFilled;

        [JsonProperty("cumQty")]
        public decimal FulfilledQuantity;

        [JsonProperty("origQty")]
        public decimal Quantity;

        [JsonProperty("updateTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime;

        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime;

        [JsonProperty("orderId")]
        public long OrderId;

        public long? TradeId;

        [JsonProperty("symbol")]
        public Symbol? Symbol;

        [JsonProperty("side"), JsonConverter(typeof(OrderPositionConverter))]
        public OrderSide? Side;

        [JsonProperty("status"), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus? Status;

        [JsonProperty("closePosition")]
        public bool Final;
    }
}
