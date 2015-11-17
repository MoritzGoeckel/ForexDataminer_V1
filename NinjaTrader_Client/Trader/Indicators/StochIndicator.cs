using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Indicators
{
    class StochIndicator : Indicator
    {
        private int timeframe;

        public StochIndicator(Database database, int timeframe) : base(database)
        {
            this.timeframe = timeframe;
        }

        //Todo: Caching, Abstract
        //Maybe just save the data to save cpu?
        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            List<Tickdata> data = database.getPrices(timestamp - timeframe, timestamp, instrument); //Flaschenhals ???

            if (data.Count == 0)
                return null;

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach(Tickdata tick in data)
            {
                if (tick.ask > max)
                    max = tick.ask;

                if (tick.bid < min)
                    min = tick.bid;
            }

            Tickdata lastTick = data[data.Count - 1];

            double now = lastTick.ask;

            return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            List<TimeValueData> data = database.getDataInRange(timestamp - timeframe, timestamp, dataName, instrument); //Flaschenhals ???

            if (data.Count == 0)
                return null;

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (TimeValueData tick in data)
            {
                if (tick.value > max)
                    max = tick.value;

                if (tick.value < min)
                    min = tick.value;
            }

            TimeValueData lastTick = data[data.Count - 1];

            double now = lastTick.value;

            return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
        }
    }
}
