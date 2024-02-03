﻿using Binance.Net.Enums;
using PMM.Core.Provider.Enum;


namespace PMM.Core.Utils
{
    internal static class ExtensionMethod
    {
        public static Interval ToInterval(this KlineInterval interval)
        {
            return interval switch
            {
                KlineInterval.OneSecond => Interval.OneSecond,
                KlineInterval.OneMinute => Interval.OneMinute,
                KlineInterval.ThreeMinutes => Interval.ThreeMinutes,
                KlineInterval.FiveMinutes => Interval.FiveMinutes,
                KlineInterval.FifteenMinutes => Interval.FifteenMinutes,
                KlineInterval.ThirtyMinutes => Interval.ThirtyMinutes,
                KlineInterval.OneHour => Interval.OneHour,
                KlineInterval.TwoHour => Interval.TwoHour,
                KlineInterval.FourHour => Interval.FourHour,
                KlineInterval.SixHour => Interval.SixHour,
                KlineInterval.EightHour => Interval.EightHour,
                KlineInterval.TwelveHour => Interval.TwelveHour,
                KlineInterval.OneDay => Interval.OneDay,
                KlineInterval.ThreeDay => Interval.ThreeDay,
                KlineInterval.OneWeek => Interval.OneWeek,
                KlineInterval.OneMonth => Interval.OneMonth,
                _ => throw new NotImplementedException(),
            };
        }

        public static KlineInterval ToBinanceInterval(this Interval interval)
        {
            return interval switch
            {
                Interval.OneSecond => KlineInterval.OneSecond,
                Interval.OneMinute => KlineInterval.OneMinute,
                Interval.ThreeMinutes => KlineInterval.ThreeMinutes,
                Interval.FiveMinutes => KlineInterval.FiveMinutes,
                Interval.FifteenMinutes => KlineInterval.FifteenMinutes,
                Interval.ThirtyMinutes => KlineInterval.ThirtyMinutes,
                Interval.OneHour => KlineInterval.OneHour,
                Interval.TwoHour => KlineInterval.TwoHour,
                Interval.FourHour => KlineInterval.FourHour,
                Interval.SixHour => KlineInterval.SixHour,
                Interval.EightHour => KlineInterval.EightHour,
                Interval.TwelveHour => KlineInterval.TwelveHour,
                Interval.OneDay => KlineInterval.OneDay,
                Interval.ThreeDay => KlineInterval.ThreeDay,
                Interval.OneWeek => KlineInterval.OneWeek,
                Interval.OneMonth => KlineInterval.OneMonth,
                _ => throw new NotImplementedException(),
            };
        }

    }
}
