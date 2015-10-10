using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Model
{
    public class TimeValueData
    {
        public TimeValueData(long timestamp, double value)
        {
            this.timestamp = timestamp;
            this.value = value;
        }

        public long timestamp;
        public double value;
    }
}
