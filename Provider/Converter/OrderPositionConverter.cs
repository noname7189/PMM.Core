using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
namespace PMM.Core.Provider.Converter
{
    public class OrderPositionConverter : JsonConverter
    {
        private static readonly Dictionary<OrderPosition, string> values = new()
        {
            { OrderPosition.Long, "BUY" },
            { OrderPosition.Short, "SELL"},
        };

        public static string GetValue(OrderPosition value)
        {
            return values[value];
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderPosition);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == (string)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(values[(OrderPosition)value]);
        }

    }
}
