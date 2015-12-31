using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class AverageChangeIndicator : Indicator
    {
        private long timeframe, resolution;
        
        public AverageChangeIndicator(Database database, long timeframe, long resolution) : base(database)
        {
            this.timeframe = timeframe;
            this.resolution = resolution;
        }

        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            List<Tickdata> data = database.getPrices(timestamp - timeframe, timestamp, instrument);

            if (data == null || data.Count == 0)
                return new TimeValueData(timestamp, 0);

            double change = 0;
            double lastValue = data[0].last;
            long nextTimestamp = timestamp - timeframe;

            foreach (Tickdata tick in data)
            {
                if (tick.timestamp > nextTimestamp)
                {
                    change += Math.Abs(tick.last - lastValue);
                    lastValue = tick.last;

                    nextTimestamp += resolution;
                }
            }
            
            return new TimeValueData(timestamp, change / (timeframe / resolution));
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            List<TimeValueData> data = database.getDataInRange(timestamp - timeframe, timestamp, dataName, instrument);

            if (data == null || data.Count == 0)
                return new TimeValueData(timestamp, 0);

            double change = 0;
            double lastValue = data[0].value;
            long nextTimestamp = timestamp - timeframe;

            foreach (TimeValueData tick in data)
            {
                if (tick.timestamp > nextTimestamp)
                {
                    change += Math.Abs(tick.value - lastValue);
                    lastValue = tick.value;

                    nextTimestamp += resolution;
                }
            }

            return new TimeValueData(timestamp, change / (timeframe / resolution));
        }
    }
}
