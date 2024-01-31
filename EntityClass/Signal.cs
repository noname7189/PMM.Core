using Microsoft.EntityFrameworkCore;
using PMM.Core.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PMM.Core.EntityClass
{
    [Index(nameof(StartTime), IsUnique = true)]
    [Index(nameof(EndTime))]
    public class Signal
    {
        [Key] 
        public int Id { get; set; }
        [Required] 
        public DateTime StartTime { get; set; }
        public SignalType SignalType { get; set; }
        public DateTime? EndTime { get; set; }

        public decimal? EnterPrice { get; set; }
        public decimal? ExitPrice { get; set; }
        public decimal? ExpectedProfit { get; set; }
        public decimal TakeProfitPrice { get; set; }
        public decimal LosscutPrice { get; set; }

        [JsonIgnore][Required][ForeignKey(nameof(Candle))]
        public int CandleId { get; set; }
        [JsonIgnore]
        public virtual OHLCV Candle { get; set; }
    }
}
