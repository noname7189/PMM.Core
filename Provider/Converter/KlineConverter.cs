using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PMM.Core.Provider.DataClass.Rest;

namespace PMM.Core.Provider.Converter
{
    internal class KlineConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            KlineData entry = new()
            {
                StartTime = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)arr[0]),
                Open = (decimal)arr[1],
                High = (decimal)arr[2],
                Low = (decimal)arr[3],
                Close = (decimal)arr[4],
                Volume = (decimal)arr[5],
                TradeCount = (int)arr[8]
            };

            return entry;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
