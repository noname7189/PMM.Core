using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PMM.Core.EntityClass
{
    [Index(nameof(Time), IsUnique = true)]
    public class OHLCV
    {
        [Key]
        public int Id { get; set; }
        public required DateTime Time { get; set; }
        public required decimal Open { get; set; }
        public required decimal High { get; set; }
        public required decimal Low { get; set; }
        public required decimal Close { get; set; }
        public required decimal Volume { get; set; }
    }
}
