using PMM.Core.Provider.Enum;
using PMM.Core.Utils;

namespace PMM.Core.Provider.Converter.DependentConverter
{
    public class IntervalConverter : BaseConverter<Interval>
    {
        private static readonly List<KeyValuePair<Interval, string>> Values =
        [
            new KeyValuePair<Interval, string>(Interval.OneMinute, "1m"),
            new KeyValuePair<Interval, string>(Interval.ThreeMinutes, "3m"),
            new KeyValuePair<Interval, string>(Interval.FiveMinutes, "5m"),
            new KeyValuePair<Interval, string>(Interval.FifteenMinutes, "15m"),
            new KeyValuePair<Interval, string>(Interval.ThirtyMinutes, "30m"),
            new KeyValuePair<Interval, string>(Interval.OneHour, "1h"),
            new KeyValuePair<Interval, string>(Interval.TwoHour, "2h"),
            new KeyValuePair<Interval, string>(Interval.FourHour, "4h"),
            new KeyValuePair<Interval, string>(Interval.SixHour, "6h"),
            new KeyValuePair<Interval, string>(Interval.EightHour, "8h"),
            new KeyValuePair<Interval, string>(Interval.TwelveHour, "12h"),
            new KeyValuePair<Interval, string>(Interval.OneDay, "1d"),
            new KeyValuePair<Interval, string>(Interval.ThreeDay, "3d"),
            new KeyValuePair<Interval, string>(Interval.OneWeek, "1w"),
            new KeyValuePair<Interval, string>(Interval.OneMonth, "1M"),
        ];

        public override List<KeyValuePair<Interval, string>> Mapping => Values;

        public static string? GetValue(Interval value)
        {
            return Values.SingleOrNull(v => v.Key == value)?.Value;
        }

    }
}
