using Binance.Net.Objects.Models.Futures.Socket;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class BalanceInfo
    {
        public BalanceInfo(BinanceFuturesStreamBalance balance)
        {
            Asset = balance.Asset;
            WalletBalance = balance.WalletBalance;
            CrossWalletBalance = balance.CrossWalletBalance;
            BalanceChange = balance.BalanceChange;
        }

        public string Asset;
        public decimal WalletBalance;
        public decimal CrossWalletBalance;
        public decimal BalanceChange;
    }
}
