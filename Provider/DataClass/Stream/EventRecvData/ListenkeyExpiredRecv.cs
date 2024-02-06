

using Newtonsoft.Json;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Stream.EventRecvData
{
    public class ListenkeyExpiredRecv
    {
        [JsonProperty("stream")]
        public string ListenKey;

        [JsonProperty("data")]
        public ExpiredData Data;
    }

    public struct ExpiredData
    {
        [JsonProperty("e")]
        [JsonConverter(typeof(StreamEventTypeConverter))]
        public StreamEventType? EventType;

        [JsonProperty("E")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime;

        [JsonProperty("listenkey")]
        public string ListenKey;
    }
}
