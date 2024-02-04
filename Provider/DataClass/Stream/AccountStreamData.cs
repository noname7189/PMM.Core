using Binance.Net.Objects.Models.Futures.Socket;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Impl;
using System.Diagnostics.CodeAnalysis;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class AccountStreamData
    {
        [SetsRequiredMembers]
        public AccountStreamData(BinanceFuturesStreamAccountUpdateData data, DateTime transactionTime)
        {
            TransactionTime = transactionTime;
            Reason = JKorfProvider.ReasonConverter(data.Reason);
            Balances = [.. data.Balances.Select(a => new BalanceInfo(a))];
            Positions = [.. data.Positions.Select(a => new PositionInfo(a))];
        }
        public required DateTime TransactionTime;
        public required UpdateReason Reason;
        public required List<BalanceInfo> Balances;
        public required List<PositionInfo> Positions;
    }
}
