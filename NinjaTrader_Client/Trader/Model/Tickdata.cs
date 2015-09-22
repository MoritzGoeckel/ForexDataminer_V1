using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Model
{
    public class Tickdata
    {
        public long timestamp;
        public double bid, ask, last;

        public Tickdata(long timestamp, double last, double bid, double ask)
        {
            this.timestamp = timestamp;
            this.bid = bid;
            this.ask = ask;
            this.last = last;
        }
    }
}
