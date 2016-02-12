using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NinjaTrader_Client.Trader.Indicators
{
    public abstract class Indicator
    {
        public Database database;

        public Indicator(Database database)
        {
            this.database = database;
        }

        public abstract TimeValueData getIndicator(long timestamp, string instrument);
        public abstract TimeValueData getIndicator(long timestamp, string dataName, string instrument);
    }
}
