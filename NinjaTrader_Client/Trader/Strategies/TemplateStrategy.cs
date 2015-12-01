using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public class TemplateStrategy : Strategy
    {
        int version = 0;

        private double percentTakeprofit, percentStoploss;
        //private bool follow_trend;
        private int hitSL = 0, hitTP = 0;

        public TemplateStrategy(Database database, double percentStoploss, double percentTakeprofit)
            : base(database)
        {
            this.percentStoploss = percentStoploss;
            this.percentTakeprofit = percentTakeprofit;
        }

        public TemplateStrategy(Database database, Dictionary<string, string> parameters) 
            : base(database)
        {
            percentStoploss = Double.Parse(parameters["sl"]);
            percentTakeprofit = Double.Parse(parameters["tp"]);
        }

        public override Strategy copy()
        {
            return new TemplateStrategy(database, percentStoploss, percentTakeprofit);
        }

        public override string getName()
        {
            return "Template-Strategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tp", percentTakeprofit.ToString());
            parameters.Add("sl", percentStoploss.ToString());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("hitSl", hitSL.ToString());
            result.Add("hitTp", hitTP.ToString());

            return result;
        }

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            double takeprofit = api.getAvgPrice(instrument) * percentTakeprofit / 100d;
            double stoploss = api.getAvgPrice(instrument) * percentStoploss / 100d;

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));

            if(api.getShortPosition(instrument) == null && api.getLongPosition(instrument) != null)
            {

            }

            if (api.getShortPosition(instrument).getDifference() > takeprofit)
                api.closeShort(instrument);

            if (api.getLongPosition(instrument).getDifference() > takeprofit)
                api.closeLong(instrument);

            if (api.getShortPosition(instrument).getDifference() < -stoploss)
                api.closeShort(instrument);

            if (api.getLongPosition(instrument).getDifference() > -stoploss)
                api.closeLong(instrument);
        }
    }
}
