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
    public class SSIStochStrategy : Strategy
    {
        StochIndicator stochIndicator;
        private Dictionary<string, List<TimeValueData>> lastStochTicks = new Dictionary<string, List<TimeValueData>>();
        private double takeprofitPercent, threshold, stoplossPercent;
        private int stochTimeframe, timeout;
        private bool againstCrowd;

        private int hitTimeout = 0, hitTp = 0, hitSl = 0;

        int version = 0;

        public SSIStochStrategy(Database database, double takeprofitPercent, double stoplossPercent, double threshold, int timeout, int stochTimeframe, bool againstCrowd)
            : base(database)
        {
            this.takeprofitPercent = takeprofitPercent;
            this.threshold = threshold;
            this.timeout = timeout;
            this.stochTimeframe = stochTimeframe;
            this.stoplossPercent = stoplossPercent;
            this.againstCrowd = againstCrowd;

            stochIndicator = new StochIndicator(database, stochTimeframe);
        }

        public SSIStochStrategy(Database database, Dictionary<string, string> parameters)
            : base(database)
        {
            stochTimeframe = Convert.ToInt32(parameters["stochT"]);
            timeout = Convert.ToInt32(parameters["to"]);
            threshold = Double.Parse(parameters["threshold"]);
            takeprofitPercent = Double.Parse(parameters["tp"]);
            stoplossPercent = Double.Parse(parameters["sl"]);
            againstCrowd = Boolean.Parse(parameters["againstCrowd"]);

            stochIndicator = new StochIndicator(database, stochTimeframe);
        }

        public override Strategy copy()
        {
            return new SSIStochStrategy(database, takeprofitPercent, stoplossPercent, threshold, timeout, stochTimeframe, againstCrowd);
        }

        public override string getName()
        {
            return "SSIStochStrategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tp", takeprofitPercent.ToString());
            parameters.Add("threshold", threshold.ToString());
            parameters.Add("to", timeout.ToString());
            parameters.Add("stochT", stochTimeframe.ToString());
            parameters.Add("sl", stoplossPercent.ToString());
            parameters.Add("againstCrowd", againstCrowd.ToString());

            parameters.Add("name", getName());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("hitTo", hitTimeout.ToString());
            result.Add("hitTp", hitTp.ToString());
            result.Add("hitSl", hitSl.ToString());

            return result;
        }

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;
            
            double takeprofit = api.getAvgPrice(instrument) * takeprofitPercent / 100d;
            double stoploss = api.getAvgPrice(instrument) * stoplossPercent / 100d;

            TimeValueData stockTick = stochIndicator.getIndicator(api.getNow(), "ssi-mt4", instrument);
            
            if (stockTick == null)
                return;

            if (lastStochTicks.ContainsKey(instrument) == false)
                lastStochTicks.Add(instrument, new List<TimeValueData>());

            lastStochTicks[instrument].Add(stockTick);

            //Liste x Minuten in die Vergangenheit um zu sehen ob gerade eine grenze überschritten wurde
            while (api.getNow() - lastStochTicks[instrument][0].timestamp > 1000 * 60 * 5)
            {
                lastStochTicks[instrument].RemoveAt(0);

                if (lastStochTicks[instrument].Count == 0)
                    return;
            }

            double stochNow = lastStochTicks[instrument][0].value;
            double stochMinInTimeframe = double.MaxValue;
            double stochMaxInTimeframe = double.MinValue;
            foreach (TimeValueData stoch in lastStochTicks[instrument])
            {
                if (stoch.value > stochMaxInTimeframe)
                    stochMaxInTimeframe = stoch.value;

                if (stoch.value < stochMinInTimeframe)
                    stochMinInTimeframe = stoch.value;
            }

            //wenn noch keine position besteht
            if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) == null)
            {
                if (stochMaxInTimeframe > (1 - threshold) && stochNow <= (1 - threshold))
                {
                    if (againstCrowd)
                        api.openLong(instrument);
                    else
                        api.openShort(instrument);
                }

                if (stochMinInTimeframe < threshold && stochNow > threshold)
                {
                    if (againstCrowd)
                        api.openShort(instrument);
                    else
                        api.openLong(instrument);
                }
            }

            if (api.getLongPosition(instrument) != null)
            {
                if (takeprofit != 0 && api.getLongPosition(instrument).getDifference(api.getBid(instrument), api.getAsk(instrument)) > takeprofit)
                {
                    api.closePositions(instrument);
                    hitTp++;
                }
                else if (api.getNow() - api.getLongPosition(instrument).timestampOpen > timeout)
                {
                    api.closePositions(instrument);
                    hitTimeout++;
                }
                else if(stoploss != 0 && api.getLongPosition(instrument).getDifference(api.getBid(instrument), api.getAsk(instrument)) < -stoploss)
                {
                    api.closePositions(instrument);
                    hitSl++;
                }
            }

            if (api.getShortPosition(instrument) != null)
            {
                if (takeprofit != 0 && api.getShortPosition(instrument).getDifference(api.getBid(instrument), api.getAsk(instrument)) > takeprofit)
                {
                    api.closePositions(instrument);
                    hitTp++;
                }
                else if (api.getNow() - api.getShortPosition(instrument).timestampOpen > timeout)
                {
                    api.closePositions(instrument);
                    hitTimeout++;
                }
                else if (stoploss != 0 && api.getShortPosition(instrument).getDifference(api.getBid(instrument), api.getAsk(instrument)) < -stoploss)
                {
                    api.closePositions(instrument);
                    hitSl++;
                }
            }
        }
    }
}
