using Newtonsoft.Json;
using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    public class GoodTillDateConverter : BaseConverter<GoodTillDate>
    {
        private static readonly List<KeyValuePair<GoodTillDate, string>> Values =
        [
            new KeyValuePair<GoodTillDate, string>(GoodTillDate.GoodTillCanceled, "GTC"),
            new KeyValuePair<GoodTillDate, string>(GoodTillDate.ImmediateOrCancel, "IOC"),
            new KeyValuePair<GoodTillDate, string>(GoodTillDate.FillOrKill, "FOK"),
            new KeyValuePair<GoodTillDate, string>(GoodTillDate.GoodTillCrossing, "GTX"),
            new KeyValuePair<GoodTillDate, string>(GoodTillDate.GoodTillExpiredOrCanceled, "GTE_GTC"),
        ];

        public override List<KeyValuePair<GoodTillDate, string>> Mapping => Values;

        public static string? GetValue(GoodTillDate value)
        {
            return Values.Single(v => v.Key == value).Value;
        }

    }
}
