﻿namespace PMM.Core.Provider.DataClass.Stream
{
    public class KlineStreamData
    {
        public required decimal Open;
        public required decimal High;
        public required decimal Low;
        public required decimal Close;
        public required decimal Volume;
        public required decimal QuoteVolume;
        public required DateTime StartTime;
        public required DateTime EndTime;

        public required int TradeCount;
        public required bool Final;
    }
}
