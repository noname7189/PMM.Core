using PMM.Core.EntityClass;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Binance.Net.Enums;

namespace PMM.Core.DataClass
{
    public class OnlineOrder
    {
        public long OrderId { get; set; }
        public long? CounterOrderId { get; set; }
        public long? TradeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FulfilledQuantity { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus OrderStatus { get; set; }

        [JsonIgnore]
        [Required]
        [ForeignKey(nameof(Signal))]
        public int SignalId { get; set; }
        [JsonIgnore]
        public virtual BaseSignal Signal { get; set; }
    }
}
