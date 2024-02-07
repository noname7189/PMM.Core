using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    internal class OrderStatusConverter : BaseConverter<OrderStatus>
    {
        private static List<KeyValuePair<OrderStatus, string>> Values => [
        
            new KeyValuePair<OrderStatus, string>(OrderStatus.New, "NEW"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.PartiallyFilled, "PARTIALLY_FILLED"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.Filled, "FILLED"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.Canceled, "CANCELED"),
        ];

        public override List<KeyValuePair<OrderStatus, string>> Mapping => Values;

        public static string? GetValue(OrderStatus value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }
    }
}
