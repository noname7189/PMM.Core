using CryptoExchange.Net.Attributes;

namespace PMM.Core.Provider.Enum
{
    public enum Interval
    {
        OneSecond = 1,
        OneMinute = 60,
        ThreeMinutes = 180, //60 * 3
        FiveMinutes = 300, // 60 * 5
        FifteenMinutes = 900, //60 * 15
        ThirtyMinutes = 1800, //60 * 30       
        OneHour = 3600, //60 * 60
        TwoHour = 7200, //60 * 60 * 2,
        FourHour = 14400, // 60 * 60 * 4,
        SixHour = 21600, //60 * 60 * 6,
        EightHour = 28800, //60 * 60 * 8,
        TwelveHour = 43200, //60 * 60 * 12,
        OneDay = 86400, //60 * 60 * 24,
        ThreeDay = 259200, //60 * 60 * 24 * 3,
        OneWeek = 604800, //60 * 60 * 24 * 7,
        OneMonth = 2592000, //60 * 60 * 24 * 30
    }
}
