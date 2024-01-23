
using PMM.Core.Utils;

namespace PMM.Core.DataClass
{
    public class OnlineSignal
    {
        public OnlineSignal() 
        {
            SignalId = Util.AppSignalId;
        }
        public int SignalId { get; }
        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public decimal? EnterPrice { get; set; }
        public decimal? ExitPrice { get; set; }
        public decimal? ExpectedProfit { get; set; }

        public required decimal TakeProfitPrice { get; set; }
        public required decimal LosscutPrice { get; set; }
        public override bool Equals(object? obj)
        {
            return SignalId == (obj as OnlineSignal)?.SignalId;
        }

        public override int GetHashCode()
        {
            return SignalId.GetHashCode();
        }
    }
}
