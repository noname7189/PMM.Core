using Binance.Net.Enums;
using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    internal class OrderStatusConverter : BaseConverter<OrderStatusType>
    {
        private static List<KeyValuePair<OrderStatusType, string>> Values => [
        
            new KeyValuePair<OrderStatusType, string> (OrderStatusType.New, "NEW"),
            new KeyValuePair<OrderStatusType, string> (OrderStatusType.PartiallyFilled, "PARTIALLY_FILLED"),
            new KeyValuePair<OrderStatusType, string> (OrderStatusType.Filled, "FILLED"),
            new KeyValuePair<OrderStatusType, string> (OrderStatusType.Canceled, "CANCELED"),
        ];

        public override List<KeyValuePair<OrderStatusType, string>> Mapping => Values;

        public static string? GetValue(OrderStatusType value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }
    }
}
