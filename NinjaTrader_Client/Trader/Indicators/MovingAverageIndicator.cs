using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class MovingAverageIndicator : Indicator
    {
        private long timeframe;

        public MovingAverageIndicator(Database database, long timeframe) : base(database)
        {
            this.timeframe = timeframe;
        }

        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            throw new NotImplementedException();
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            throw new NotImplementedException();
        }
    }
}
