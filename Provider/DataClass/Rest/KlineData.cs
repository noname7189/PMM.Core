using Newtonsoft.Json;
using PMM.Core.Provider.Converter;

namespace PMM.Core.Provider.DataClass.Rest
{
    [JsonConverter(typeof(KlineConverter))]
    public record KlineData
    {
        public required DateTime StartTime;
        public required decimal Open;
        public required decimal High;
        public required decimal Low;
        public required decimal Close;
        public required decimal Volume;
        public required int TradeCount;
    }
}
