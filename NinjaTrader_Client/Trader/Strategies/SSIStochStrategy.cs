using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Indicators;
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
        private List<TimeValueData> stochs = new List<TimeValueData>();
        private double takeprofit, threshold;
        private int stochTimeframe, timeout;

        public SSIStochStrategy(Database database, double takeprofit = 0.001, double threshold = 0.2, int timeout = 1000 * 60 * 30, int stochTimeframe = 1000 * 60 * 60) : base(database)
        {
            stochIndicator = new StochIndicator(database, stochTimeframe);
            this.takeprofit = takeprofit;
            this.threshold = threshold;
            this.timeout = timeout;
            this.stochTimeframe = stochTimeframe;
        }

        public override Strategy copy()
        {
            return new SSIStochStrategy(database, takeprofit, threshold, timeout, stochTimeframe);
        }

        public override string getName()
        {
            return "SSIStochStrategy";
        }

        public override BacktestResult addCustomVariables(BacktestResult given)
        {
            given.setParameter("takeprofit", takeprofit.ToString());
            given.setParameter("threshold", threshold.ToString());
            given.setParameter("timeout", (timeout / 1000d / 60d).ToString());
            given.setParameter("stochTimeframe", (stochTimeframe / 1000d / 60d).ToString());

            return given;
        }

        public override void reset()
        {
            //Reset stats
        }

        public override void doTick(string instrument)
        {
            if (api.isUptodate(instrument) == false)
                return;

            TimeValueData newestTick = stochIndicator.getIndicator(api.getNow(), "ssi-mt4", instrument);
            
            if (newestTick == null)
                return;

            stochs.Add(newestTick);

            while (api.getNow() - stochs[0].timestamp > 1000 * 60 * 3)
            {//Liste 3 Minuten in die Vergangenheit
                stochs.RemoveAt(0);

                if (stochs.Count == 0)
                    return;
            }

            double stochNow = stochs[0].value;
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach(TimeValueData stoch in stochs)
            {
                if (stoch.value > max)
                    max = stoch.value;

                if (stoch.value < min)
                    min = stoch.value;
            }

            if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) == null)
            {
                if (max > (1 - threshold) && stochNow <= (1 - threshold))
                {
                    api.openLong(instrument);
                }

                if (min < threshold && stochNow > threshold)
                {
                    api.openShort(instrument);
                }
            }

            //collect stats and give them to the result
            if (api.getLongPosition(instrument) != null &&
                (api.getBid(instrument) - api.getLongPosition(instrument).priceOpen > takeprofit || api.getNow() - api.getLongPosition(instrument).timestampOpen > timeout))
            {
                api.closePositions(instrument);
            }

            if (api.getShortPosition(instrument) != null &&
                (api.getShortPosition(instrument).priceOpen - api.getAsk(instrument) > takeprofit || api.getNow() - api.getShortPosition(instrument).timestampOpen > timeout))
            {
                api.closePositions(instrument);
            }
        }
    }
}
