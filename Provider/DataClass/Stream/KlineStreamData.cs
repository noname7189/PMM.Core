using Newtonsoft.Json;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class KlineStreamData
    {
        [JsonProperty("o")]
        public decimal Open;

        [JsonProperty("h")]
        public decimal High;

        [JsonProperty("l")]
        public decimal Low;

        [JsonProperty("c")]
        public decimal Close;

        [JsonProperty("v")]
        public decimal Volume;

        [JsonProperty("t")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime StartTime;

        [JsonProperty("T")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime EndTime;

        [JsonProperty("n")]
        public int TradeCount;

        [JsonProperty("x")]
        public bool Final;
    }
}
