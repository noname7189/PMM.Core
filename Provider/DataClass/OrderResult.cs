using PMM.Core.Enum;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass
{
    public class OrderResult
    {
        public required long OrderId;
        public required Symbol Symbol;
        public required OrderPosition Side;
        public required OrderStatusType Status;

        public required decimal Price;
        public required decimal AveragePrice;

        public required decimal QuantityFilled;
        public required decimal? CummulativeQuantity;
        public required decimal Quantity;

        public required bool Final;
        public required DateTime UpdateTime;
        public required DateTime CreateTime;
    }
}
