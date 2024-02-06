using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    internal class UpdateReasonConverter : BaseConverter<UpdateReason>
    {
        private static readonly List<KeyValuePair<UpdateReason, string>> Values =
            [
                new KeyValuePair<UpdateReason, string>(UpdateReason.FundingFee, "FUNDING_FEE"),
            ];
        public override List<KeyValuePair<UpdateReason, string>> Mapping => Values;

        public static string? GetValue(UpdateReason value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }

        public static UpdateReason? GetKey(string? key)
        {
            return Values.SingleOrNull(v => v.Value == key)?.Key;
        }

    }
}
