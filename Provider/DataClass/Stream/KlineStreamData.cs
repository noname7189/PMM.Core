using Newtonsoft.Json;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class KlineStreamData : KlineStreamRawData
    {
        [JsonProperty("t")]
        [JsonConverter(typeof(DateTimeConverter))]
        public new DateTime StartTime;

        [JsonProperty("T")]
        [JsonConverter(typeof(DateTimeConverter))]
        public new DateTime EndTime;
    }
}
