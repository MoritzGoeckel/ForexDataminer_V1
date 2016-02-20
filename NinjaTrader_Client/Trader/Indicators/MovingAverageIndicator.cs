using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class MovingAverageIndicator : WalkerIndicator
    {
        private long timeframe;
        private List<TimestampValuePair> history = new List<TimestampValuePair>();

        public MovingAverageIndicator(long timeframe)
        {
            this.timeframe = timeframe;
        }

        double valueNow;
        long timestampNow;
        public override void setNextData(long _timestamp, double _value)
        {
            if (_timestamp < timestampNow)
                throw new Exception("Cant add older data here!");

            if (_timestamp == timestampNow && _value != valueNow)
                throw new Exception("Same timestamp different value!");

            if (_timestamp == timestampNow && _value == valueNow)
                return;

            history.Add(new TimestampValuePair { timestamp = _timestamp, value = _value });
            timestampNow = _timestamp;
            valueNow = _value;

            sum += _value;
        }

        double sum = 0;
        public override TimeValueData getIndicator()
        {
            while (history.Count != 0 && history[0].timestamp < timestampNow - timeframe)
            {
                sum -= history[0].value;
                history.RemoveAt(0);
            }

            double count = history.Count;

            return new TimeValueData(timestampNow, (valueNow - (sum / count)) / valueNow * 100); //Percent difference... -1 = Preis 1% unter MA && 1 = Preis 1% über MA
        }

        public override string getName()
        {
            return "MA_" + timeframe;
        }
    }
}
