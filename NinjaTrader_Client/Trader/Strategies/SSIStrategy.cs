using NinjaTrader_Client.Trader.Indicators;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class SSIStrategy : Strategy
    {
        double thresholdOpen, thresholdClose;
        bool followCrowd;

        int version = 1;

        private Indicator tradingTime;

        public SSIStrategy(Database database, double thresholdOpen, double thresholdClose, bool followCrowd)
            : base(database)
        {
            this.thresholdOpen = thresholdOpen;
            this.thresholdClose = thresholdClose;
            this.followCrowd = followCrowd;

            this.tradingTime = new TradingTimeIndicator(database);
        }

        public SSIStrategy(Database database, Dictionary<string, string> parameters) : base(database)
        {
            thresholdOpen = Double.Parse(parameters["thresholdOpen"]);
            thresholdClose = Double.Parse(parameters["thresholdClose"]);
            followCrowd = Boolean.Parse(parameters["followCrowd"]);
        }

        public override Strategy copy()
        {
            return new SSIStrategy(database, thresholdOpen, thresholdClose, followCrowd);
        }

        public override string getName()
        {
            return "SSIStrategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("thresholdOpen", thresholdOpen.ToString());
            parameters.Add("thresholdClose", thresholdClose.ToString());
            parameters.Add("followCrowd", followCrowd.ToString());

            parameters.Add("name", getName());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            return result;
        }

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double tradingTimeCode = tradingTime.getIndicator(api.getNow(), instrument).value;
            if (tradingTimeCode == 0)
            {
                //Flatten everything
                api.closePositions(instrument);
                return;
            }

            TimeValueData ssiValue = database.getData(api.getNow(), "ssi-mt4", instrument);

            if (ssiValue == null)
                return;

            double ssi = ssiValue.value;

            if (tradingTimeCode == 1)
            {
                //SSI im long
                if (ssi > thresholdOpen)
                {
                    if (followCrowd == false && api.getShortPosition(instrument) == null)
                        api.openShort(instrument);

                    if (followCrowd && api.getLongPosition(instrument) == null)
                        api.openLong(instrument);
                }

                //SSI im short
                if (ssi < -thresholdOpen)
                {
                    if (followCrowd == false && api.getLongPosition(instrument) == null)
                        api.openLong(instrument);

                    if (followCrowd && api.getShortPosition(instrument) == null)
                        api.openShort(instrument);
                }
            }

            //bin im Long
            if (api.getLongPosition(instrument) != null)
                if (followCrowd == false)
                {
                    if(ssi > -thresholdClose)
                        api.closePositions(instrument);
                }
                else
                {
                    if(ssi < thresholdClose)
                        api.closePositions(instrument);
                }

            //Bin im short
            if (api.getShortPosition(instrument) != null)
                if (followCrowd == false)
                {
                    if(ssi < thresholdClose)
                        api.closePositions(instrument);
                }
                else
                {
                    if(ssi > -thresholdClose)
                        api.closePositions(instrument);
                }
        }
    }
}
