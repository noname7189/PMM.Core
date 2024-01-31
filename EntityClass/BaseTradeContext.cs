
using System.ComponentModel.DataAnnotations;

namespace PMM.Core.EntityClass
{
    public class BaseTradeContext
    {
        [Key]
        public int Id { get; set; }
        public decimal CurrentMargin { get; set; }
        public decimal InitialMargin { get; set; }
        public decimal CurrentMarginRate { get; set; }
        public decimal CurrentDeposit { get; set; }
    }
}
