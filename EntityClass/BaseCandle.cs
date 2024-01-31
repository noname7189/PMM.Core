using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PMM.Core.EntityClass
{
    [Index(nameof(Time), IsUnique = true)]
    public class BaseCandle
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
