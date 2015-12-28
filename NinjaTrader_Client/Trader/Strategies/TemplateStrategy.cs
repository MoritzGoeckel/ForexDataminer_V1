using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Indicators;
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

        private Indicator tradingTime;

        public TemplateStrategy(Database database, double percentStoploss, double percentTakeprofit)
            : base(database)
        {
            this.percentStoploss = percentStoploss;
            this.percentTakeprofit = percentTakeprofit;
            this.tradingTime = new TradingTimeIndicator(database);
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

            parameters.Add("name", getName());

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

            double tradingTimeCode = tradingTime.getIndicator(api.getNow(), instrument).value;
            if (tradingTimeCode == 0)
            {
                //Flatten everything
                api.closePositions(instrument);
                return;
            }

            double takeprofit = api.getAvgPrice(instrument) * percentTakeprofit / 100d;
            double stoploss = api.getAvgPrice(instrument) * percentStoploss / 100d;

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));

            //only open positons on tradingTimeCode == 1

            if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) != null)
            {

            }

            if (api.getShortPosition(instrument).getDifference() > takeprofit)
            {
                api.closeShort(instrument);
                hitTP++;
            }

            if (api.getLongPosition(instrument).getDifference() > takeprofit)
            {
                api.closeLong(instrument);
                hitTP++;
            }

            if (api.getShortPosition(instrument).getDifference() < -stoploss)
            {
                api.closeShort(instrument);
                hitSL++;
            }

            if (api.getLongPosition(instrument).getDifference() < -stoploss)
            {
                api.closeLong(instrument);
                hitSL++;
            }
        }
    }
}
