using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;
using PMM.Core.Provider.Enum;
using PMM.Core.Provider.Impl;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class AccountStreamRecv : BaseStreamRecv
    {
        public AccountStreamRecv() { }
        public AccountStreamRecv(BinanceFuturesStreamAccountUpdateData data, DateTime transactionTime)
        {
            TransactionTime = transactionTime;
            Data = new()
            {
                Reason = JKorfProvider.ReasonConverter(data.Reason),
                Balances = [.. data.Balances.Select(a => new BalanceInfo(a))],
                Positions = [.. data.Positions.Select(a => new PositionInfo(a))],
            };
        }


        [JsonProperty("T")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime TransactionTime;
        [JsonProperty("a")]
        public AccountStreamData Data;
    }

    public class AccountStreamData
    {
        public AccountStreamData() { }

        [JsonProperty("m")]
        [JsonConverter(typeof(UpdateReasonConverter))]
        public UpdateReason? Reason;

        [JsonProperty("B")]
        public List<BalanceInfo> Balances;

        [JsonProperty("P")]
        public List<PositionInfo> Positions;

    }
}
