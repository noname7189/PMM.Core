using Newtonsoft.Json;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class StreamEvent
    {
        [JsonProperty("e")]
        public required string Event { get; set; } = string.Empty;
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public required DateTime EventTime { get; set; }

    }
}
