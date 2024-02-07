using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Provider.Converter.DependentConverter;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class PositionInfo
    {
        [JsonProperty("s"), JsonConverter(typeof(SymbolConverter))]
        public Symbol? Symbol { get; set; }
        [JsonProperty("pa")]
        public decimal Quantity { get; set; }
        [JsonProperty("ep")]
        public decimal EntryPrice { get; set; }
        [JsonProperty("cr")]
        public decimal RealizedPnl { get; set; }
        [JsonProperty("up")]
        public decimal UnrealizedPnl { get; set; }
    }
}
