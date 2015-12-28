using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class MovingAverage : Indicator
    {
        private long timeframe;

        public MovingAverage(Database database, long timeframe) : base(database)
        {
            this.timeframe = timeframe;
        }

        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            return new TimeValueData(timestamp, 0.5);
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            return new TimeValueData(timestamp, 0.5);
        }
    }
}
