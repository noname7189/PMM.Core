using Newtonsoft.Json;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class KlineStreamRawData
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
        public long StartTime;

        [JsonProperty("T")]
        public long EndTime;

        [JsonProperty("n")]
        public int TradeCount;

        [JsonProperty("x")]
        public bool Final;
    }
}
