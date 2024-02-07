using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;
using PMM.Core.Provider.Converter;
using PMM.Core.Provider.Converter.DependentConverter;
using PMM.Core.Provider.DataClass.Rest;
using PMM.Core.Provider.DataClass.Stream.EventRecvData;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Stream
{
    public class AccountStreamRecv : BaseStreamRecv
    {
        [JsonProperty("T")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime TransactionTime;
        [JsonProperty("a")]
        public AccountStreamData Data;
    }

    public class AccountStreamData
    {
        [JsonProperty("m")]
        [JsonConverter(typeof(UpdateReasonConverter))]
        public UpdateReason? Reason;

        [JsonProperty("B")]
        public List<BalanceInfo> Balances;

        [JsonProperty("P")]
        public List<PositionInfo> Positions;
    }
}
