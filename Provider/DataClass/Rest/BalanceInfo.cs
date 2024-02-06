using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class BalanceInfo
    {
        public BalanceInfo() { }
        public BalanceInfo(BinanceFuturesStreamBalance balance)
        {
            Asset = balance.Asset;
            WalletBalance = balance.WalletBalance;
            CrossWalletBalance = balance.CrossWalletBalance;
            BalanceChange = balance.BalanceChange;
        }

        [JsonProperty("a")]
        public string Asset;
        [JsonProperty("wb")]
        public decimal WalletBalance;
        [JsonProperty("cw")]
        public decimal CrossWalletBalance;
        [JsonProperty("bc")]
        public decimal BalanceChange;
    }
}
