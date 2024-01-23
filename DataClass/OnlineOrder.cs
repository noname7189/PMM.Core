using Binance.Net.Enums;
using PMM.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMM.Core.DataClass
{
    public class OnlineOrder
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Symbol Symbol { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KlineInterval Interval { get; set; }
        public long? TradeId { get; set; }
        public long OrderId { get; set; }
        public bool Finished { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FulfilledQuantity { get; set; }
        public int MixedSignalId { get; set; }
    }
}
