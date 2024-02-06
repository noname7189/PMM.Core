using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
using PMM.Core.Utils;
namespace PMM.Core.Provider.Converter.DependentConverter
{
    public class OrderPositionConverter : BaseConverter<OrderPosition>
    {
        private static readonly List<KeyValuePair<OrderPosition, string>> Values =
        [
            new KeyValuePair<OrderPosition, string>(OrderPosition.Long, "BUY"),
            new KeyValuePair<OrderPosition, string>(OrderPosition.Short, "SELL"),
        ];

        public override List<KeyValuePair<OrderPosition, string>> Mapping => Values;

        public static string? GetValue(OrderPosition value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }
    }
}
