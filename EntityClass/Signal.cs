using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMM.Core.EntityClass
{
    [Index(nameof(StartTime), IsUnique = true)]
    public class Signal
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public required DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public decimal? EnterPrice { get; set; }
        public decimal? ExitPrice { get; set; }
        public decimal? ExpectedProfit { get; set; }
        public decimal? TakeProfitPrice { get; set; }
        public decimal? LosscutPrice { get; set; }

        [JsonIgnore]
        [Required]
        public required int CandleId { get; set; }
        [JsonIgnore]
        public virtual OHLCV Candle { get; set; }

    }
}
