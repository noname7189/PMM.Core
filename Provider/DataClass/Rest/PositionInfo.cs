using Binance.Net.Objects.Models.Futures.Socket;
using Newtonsoft.Json;

namespace PMM.Core.Provider.DataClass.Rest
{
    public class PositionInfo
    {
        public PositionInfo() { }
        public PositionInfo(BinanceFuturesStreamPosition position) 
        {
            Symbol = position.Symbol;
            Quantity = position.Quantity;
            EntryPrice = position.EntryPrice;
            RealizedPnl = position.RealizedPnl;
            UnrealizedPnl = position.UnrealizedPnl;
        }
        [JsonProperty("s")]
        public string Symbol { get; set; }
        [JsonProperty("pa")]
        public decimal Quantity { get; set; }
        [JsonProperty("ep")]
        public decimal EntryPrice { get; set; }
        [JsonProperty("cr")]
        public decimal RealizedPnl { get; set; }
        [JsonProperty("up")]
        public decimal UnrealizedPnl { get; set; }
    }
}
