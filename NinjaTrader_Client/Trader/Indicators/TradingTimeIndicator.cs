using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Utils;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class TradingTimeIndicator : Indicator
    {
        public TradingTimeIndicator(Database database) : base(database)
        {
            
        }

        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            DateTime dt = Timestamp.getDate(timestamp);

            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                return new TimeValueData(timestamp, 0); //Kein trading

            if (dt.DayOfWeek == DayOfWeek.Friday && dt.Hour >= 22)
                return new TimeValueData(timestamp, 0); //Kein trading

            if (dt.DayOfWeek == DayOfWeek.Friday && dt.Hour >= 20)
                return new TimeValueData(timestamp, 0.5); //Do not open positions

            return new TimeValueData(timestamp, 1); //Happy trading :)
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            throw new Exception("Not implemented in TradingTimeIndicator; Use -> getIndicator(long timestamp, string instrument)");
        }
    }
}
