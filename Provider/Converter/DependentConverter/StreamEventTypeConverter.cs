using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    internal class StreamEventTypeConverter : BaseConverter<StreamEventType>
    {
        private static List<KeyValuePair<StreamEventType, string>> Values =>
        [
            new KeyValuePair<StreamEventType, string>(StreamEventType.ListenkeyExpired, "listenKeyExpired"),
            new KeyValuePair<StreamEventType, string>(StreamEventType.AccountUpdate, "ACCOUNT_UPDATE"),
            new KeyValuePair<StreamEventType, string>(StreamEventType.OrderUpdate, "ORDER_TRADE_UPDATE"),
        ];

        public override List<KeyValuePair<StreamEventType, string>> Mapping => Values;

        public static string? GetValue(StreamEventType value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }

        public static StreamEventType? GetKey(string? key)
        {
            return Values.SingleOrNull(v => v.Value == key)?.Key;
        }
    }
}
