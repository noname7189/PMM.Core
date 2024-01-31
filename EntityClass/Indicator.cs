using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PMM.Core.EntityClass
{
    [Index(nameof(Time), IsUnique = true)]
    public class Indicator
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        [JsonIgnore]
        [Required]
        [ForeignKey(nameof(Candle))]
        public int CandleId { get; set; }
        [JsonIgnore]
        public virtual OHLCV Candle { get; set; }
    }
}
