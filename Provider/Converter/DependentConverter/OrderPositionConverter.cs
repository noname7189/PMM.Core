using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
using PMM.Core.Utils;
namespace PMM.Core.Provider.Converter.DependentConverter
{
    public class OrderPositionConverter : BaseConverter<OrderSide>
    {
        private static readonly List<KeyValuePair<OrderSide, string>> Values =
        [
            new KeyValuePair<OrderSide, string>(OrderSide.Long, "BUY"),
            new KeyValuePair<OrderSide, string>(OrderSide.Short, "SELL"),
        ];

        public override List<KeyValuePair<OrderSide, string>> Mapping => Values;

        public static string? GetValue(OrderSide value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }
    }
}
