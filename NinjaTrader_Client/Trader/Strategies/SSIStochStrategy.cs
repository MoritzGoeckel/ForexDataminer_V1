using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.BacktestBase.Visualization;
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
        private Indicator stochIndicator;
        private Indicator tradingTime;
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
            tradingTime = new TradingTimeIndicator(database);

            setupVisualizationData();
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

            setupVisualizationData();
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

        private BacktestVisualizationDataComponent price_vi, uptodate_vi, tradingTimeCode_vi, ssi_vi, ssiStoch_vi, upperThreshold_vi, lowerThreshold_vi, sl_vi, tp_vi;

        public override void setupVisualizationData()
        {
            price_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("price", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
            uptodate_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("uptodate", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 2));
            tradingTimeCode_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tradingTimeCode", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 3));
            ssi_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("ssi", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 5));
            ssiStoch_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("ssiStoch", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 4));
            upperThreshold_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("upperThreshold", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 4, 1 - threshold));
            lowerThreshold_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("lowerThreshold", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 4, threshold));

            //sl_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("sl", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0)); //??? Wird nicht gesetzt ???
            //tp_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tp", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
        }

        public override void doTick(string instrument)
        {
            price_vi.value = api.getAvgPrice(instrument);

            bool isUpToDate = api.isUptodate(instrument);
            uptodate_vi.value = isUpToDate ? 1 : 0;
            if (isUpToDate == false)
                return;

            double tradingTimeCode = tradingTime.getIndicator(api.getNow(), instrument).value;
            tradingTimeCode_vi.value = tradingTimeCode;
            if (tradingTimeCode == 0)
            {
                //Flatten everything
                api.closePositions(instrument);
                return;
            }

            double takeprofit = api.getAvgPrice(instrument) * takeprofitPercent / 100d;
            double stoploss = api.getAvgPrice(instrument) * stoplossPercent / 100d;

            TimeValueData ssiValue = database.getData(api.getNow(), "ssi-mt4", instrument);
            ssi_vi.value = ssiValue.value;
            
            TimeValueData stochTick = stochIndicator.getIndicator(api.getNow(), "ssi-mt4", instrument);

            if (stochTick == null)
                return;

            ssiStoch_vi.value = stochTick.value;

            if (lastStochTicks.ContainsKey(instrument) == false)
                lastStochTicks.Add(instrument, new List<TimeValueData>());

            lastStochTicks[instrument].Add(stochTick);

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
            if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) == null && tradingTimeCode == 1)
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
