using Newtonsoft.Json;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.Converter
{
    internal class OrderTypeConverter : JsonConverter
    {
        private static readonly Dictionary<OrderType, string> values = new()
        {
            { OrderType.Limit, "LIMIT" },
            { OrderType.Market, "MARKET" },
            { OrderType.TakeProfit, "TAKE_PROFIT" },
            { OrderType.TakeProfitMarket, "TAKE_PROFIT_MARKET" },
            { OrderType.Stop, "STOP" },
            { OrderType.StopMarket, "STOP_MARKET" },
            { OrderType.TrailingStopMarket, "TRAILING_STOP_MARKET" },
            { OrderType.Liquidation, "LIQUIDATION" }
        };

        public static string GetValue(OrderType value)
        {
            return values[value];
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return values.Single(v => v.Value == (string)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(values[(OrderType)value]);
        }
    }
}
