using Newtonsoft.Json;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Converter
{
    public class IntervalConverter : JsonConverter
    {
        private static readonly Dictionary<Interval, string> values = new()
        {
            { Interval.OneMinute, "1m" },
            { Interval.ThreeMinutes, "3m" },
            { Interval.FiveMinutes, "5m" },
            { Interval.FifteenMinutes, "15m" },
            { Interval.ThirtyMinutes, "30m" },
            { Interval.OneHour, "1h" },
            { Interval.TwoHour, "2h" },
            { Interval.FourHour, "4h" },
            { Interval.SixHour, "6h" },
            { Interval.EightHour, "8h" },
            { Interval.TwelveHour, "12h" },
            { Interval.OneDay, "1d" },
            { Interval.ThreeDay, "3d" },
            { Interval.OneWeek, "1w" },
            { Interval.OneMonth, "1M" },
        };

        public static string GetValue(Interval value)
        {
            return values[value];
        }


        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Interval);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == (string)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(values[(Interval)value]);
        }

    }
}
