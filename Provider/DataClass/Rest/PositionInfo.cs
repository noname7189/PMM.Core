using Binance.Net.Objects.Models.Futures.Socket;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class PositionInfo
    {
        public PositionInfo(BinanceFuturesStreamPosition position) 
        {
            Symbol = position.Symbol;
            Quantity = position.Quantity;
            EntryPrice = position.EntryPrice;
            RealizedPnl = position.RealizedPnl;
            UnrealizedPnl = position.UnrealizedPnl;
        }
        public string Symbol { get; set; }
        public decimal Quantity { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal RealizedPnl { get; set; }
        public decimal UnrealizedPnl { get; set; }
    }
}
