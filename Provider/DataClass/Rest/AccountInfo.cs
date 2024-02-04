using Newtonsoft.Json;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class AccountInfo
    {
        public decimal TotalInitialMargin;
        public decimal TotalMarginBalance;
        public decimal TotalUnrealizedProfit;
        public decimal TotalWalletBalance;
        public decimal TotalCrossWalletBalance;
        public decimal AvailableBalance;
        [JsonProperty("updateTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? UpdateTime;
    }
}
