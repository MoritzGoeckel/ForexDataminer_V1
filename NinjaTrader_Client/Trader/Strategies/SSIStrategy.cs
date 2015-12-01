using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.MainAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class SSIStrategy : Strategy
    {
        double thresholdOpen, thresholdClose;
        bool followTrend;

        int version = 0;

        public SSIStrategy(Database database, double thresholdOpen, double thresholdClose, bool followTrend)
            : base(database)
        {
            this.thresholdOpen = thresholdOpen;
            this.thresholdClose = thresholdClose;
            this.followTrend = followTrend;
        }

        public SSIStrategy(Database database, Dictionary<string, string> parameters)
            : base(database)
        {
            thresholdOpen = Double.Parse(parameters["thresholdOpen"]);
            thresholdClose = Double.Parse(parameters["thresholdClose"]);
            followTrend = Boolean.Parse(parameters["followTrend"]);
        }

        public override Strategy copy()
        {
            return new SSIStrategy(database, thresholdOpen, thresholdClose, followTrend);
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
            parameters.Add("followTrend", followTrend.ToString());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            return result;
        }

        //private long pauseTil = 0;
        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double ssi = database.getData(api.getNow(), "ssi-mt4", instrument).value;

            if (ssi > thresholdOpen)
            {
                if (followTrend && api.getShortPosition(instrument) == null)
                    api.openShort(instrument);
                else if (followTrend == false && api.getLongPosition(instrument) == null)
                    api.openLong(instrument);
            }

            if (ssi < -thresholdOpen)
            {
                if (followTrend && api.getLongPosition(instrument) == null)
                    api.openLong(instrument);
                else if (followTrend == false && api.getShortPosition(instrument) == null)
                    api.openShort(instrument);
            }

            if (api.getLongPosition(instrument) != null && ((ssi > thresholdClose && followTrend) || (ssi < -thresholdClose && followTrend == false)))
                api.closePositions(instrument);

            if (api.getShortPosition(instrument) != null && ((ssi > thresholdClose && followTrend == false) || (ssi < -thresholdClose && followTrend)))
                api.closePositions(instrument);
        }
    }
}
