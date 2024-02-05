using Newtonsoft.Json;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.DataClass.Rest;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class SubscribeKline
    {
        [JsonProperty("E")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime;

        [JsonProperty("k")]
        public KlineData? Data;
    }
}
