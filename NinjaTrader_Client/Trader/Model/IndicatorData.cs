using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class IndicatorData
    {
        public IndicatorData(long timestamp, double value)
        {
            this.timestamp = timestamp;
            this.value = value;
        }

        public long timestamp;
        public double value;
    }
}
