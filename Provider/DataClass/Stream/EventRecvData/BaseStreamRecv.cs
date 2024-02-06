using Newtonsoft.Json;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Stream.EventRecvData
{
    public class BaseStreamRecv
    {
        [JsonProperty("e")]
        public StreamEventType? Event;
        [JsonProperty("E"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime EventTime;
    }
}
 