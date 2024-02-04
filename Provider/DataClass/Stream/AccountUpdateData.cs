using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class AccountUpdateData
    {
        public DateTime TransactionTime;
        public UpdateReason Reason;
        public List<BalanceInfo> Balances;
        public List<PositionInfo> Positions;
    }
}
