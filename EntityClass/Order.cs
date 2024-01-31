using Binance.Net.Enums;
using PMM.Core.DataClass;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMM.Core.EntityClass
{
    public class Order : OnlineOrder
    {
        [Key]
        public int Id { get; set; }
        public decimal Fee { get; set; }
        public decimal RealizedProfit { get; set; }
        public bool Finished { get; set; }
        public bool IsMaker { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
