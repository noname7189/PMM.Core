using Newtonsoft.Json;
using PMM.Core.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    internal class SymbolConverter : BaseConverter<Symbol>
    {
        private static readonly List<KeyValuePair<Symbol, string>> Values =
        [
            new KeyValuePair<Symbol, string>(Symbol.ETHUSDT, "ethusdt"),
            new KeyValuePair<Symbol, string>(Symbol.BTCUSDT, "btcusdt"),
        ];

        public override List<KeyValuePair<Symbol, string>> Mapping => Values;

        public static string? GetValue(Symbol value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }
    }
}
