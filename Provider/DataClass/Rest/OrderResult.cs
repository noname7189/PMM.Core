using PMM.Core.Enum;
using PMM.Core.Provider.Enum;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class OrderResult
    {
        public required decimal Price;
        public required decimal AveragePrice;
        public required decimal QuantityFilled;
        public required decimal FulfilledQuantity;
        public required decimal Quantity;
        public required DateTime UpdateTime;
        public required DateTime CreateTime;
        public required long OrderId;
        public required long? TradeId;
        public required Symbol Symbol;
        public required OrderPosition Side;
        public required OrderStatusType Status;
        public required bool Final;
    }
}
