using Newtonsoft.Json;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Converter
{
    public class GoodTillDateConverter : JsonConverter
    {
        private static readonly Dictionary<GoodTillDate, string> values = new()
        {
            { GoodTillDate.GoodTillCanceled, "GTC" },
            { GoodTillDate.ImmediateOrCancel, "IOC"},
            { GoodTillDate.FillOrKill, "FOK"},
            { GoodTillDate.GoodTillCrossing, "GTX"},
            { GoodTillDate.GoodTillExpiredOrCanceled, "GTE_GTC"}
        };

        public static string GetValue(GoodTillDate value)
        {
            return values[value];
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GoodTillDate);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == (string)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(values[(GoodTillDate)value]);
        }

    }
}
