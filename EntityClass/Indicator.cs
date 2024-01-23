using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMM.Core.EntityClass
{
    [Index(nameof(Time), IsUnique = true)]
    public class Indicator
    {
        [Key]
        public int Id { get; set; }
        public required DateTime Time { get; set; }
        [JsonIgnore]
        public required int CandleId { get; set; }
        [JsonIgnore]
        public virtual OHLCV Candle { get; set; }
    }
}
