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
    public class StochStrategy : Strategy
    {
        int version = 0;

        private double percentTakeprofit, percentStoploss, threshold;
        private bool followTrend;
        private int stochTimeframe;
        private int hitSL = 0, hitTP = 0;

        private WalkerIndicator stochIndicator;
        private WalkerIndicator tradingTime;

        private string instrument;
        
        public StochStrategy(Database database, string instrument, double percentStoploss, double percentTakeprofit, int stochTimeframe, double threshold, bool followTrend)
            : base(database)
        {
            this.instrument = instrument;

            this.percentStoploss = percentStoploss;
            this.percentTakeprofit = percentTakeprofit;
            this.stochTimeframe = stochTimeframe;
            this.threshold = threshold;
            this.followTrend = followTrend;

            stochIndicator = new StochIndicator(stochTimeframe);
            tradingTime = new TradingTimeIndicator();

            setupVisualizationData();
        }

        public StochStrategy(Database database, Dictionary<string, string> parameters) 
            : base(database)
        {
            percentStoploss = Double.Parse(parameters["sl"]);
            percentTakeprofit = Double.Parse(parameters["tp"]);
            stochTimeframe = Convert.ToInt32(parameters["time"]);
            followTrend = Boolean.Parse(parameters["followTrend"]);
            threshold = Double.Parse(parameters["threshold"]);

            instrument = parameters["instrument"];
            
            stochIndicator = new StochIndicator(stochTimeframe);

            setupVisualizationData();
        }

        private BacktestVisualizationDataComponent stochUpperBorder_vi, stochLowerBorder_vi, price_vi, uptodate_vi, tradingTimeCode_vi, stoch_vi, sl_vi, tp_vi;

        public override void setupVisualizationData()
        {
            stochUpperBorder_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("stochUpperBorder", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 1, 1 - threshold));
            stochLowerBorder_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("stochLowerBorder", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 1, threshold));
            price_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("price", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
            uptodate_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("uptodate", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 2));
            tradingTimeCode_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tradingTimeCode", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 3));
            stoch_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("stoch", BacktestVisualizationDataComponent.VisualizationType.ZeroToOne, 1));

            //sl_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("sl", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0)); //??? Wird nicht gesetzt ???
            //tp_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tp", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
        }

        public override Strategy copy()
        {
            return new StochStrategy(database, instrument, percentStoploss, percentTakeprofit, stochTimeframe, threshold, followTrend);
        }

        public override string getName()
        {
            return "StochStrategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("tp", percentTakeprofit.ToString());
            parameters.Add("sl", percentStoploss.ToString());
            parameters.Add("time", stochTimeframe.ToString());
            parameters.Add("threshold", threshold.ToString());
            parameters.Add("followTrend", followTrend.ToString());

            parameters.Add("instrument", instrument);

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

        private List<TimeValueData> old_Stoch = new List<TimeValueData>();

        public override void doTick()
        {
            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));
            price_vi.value = nowTick.getAvg();
            
            bool isUptodate = api.isUptodate(instrument);
            uptodate_vi.value = isUptodate ? 1 : 0;
            if (isUptodate == false)
                return;

            double tradingTimeCode = tradingTime.getIndicator(api.getNow(), 0).value;
            tradingTimeCode_vi.value = tradingTimeCode;
            if (tradingTimeCode == 0)
            {
                //Flatten everything
                api.closePositions(instrument);
                return;
            }

            double takeprofit = api.getAvgPrice(instrument) * percentTakeprofit / 100d;
            double stoploss = api.getAvgPrice(instrument) * percentStoploss / 100d;

            TimeValueData stochTick = stochIndicator.getIndicator(api.getNow(), nowTick.getAvg());
            stoch_vi.value = stochTick.value;

            while (old_Stoch.Count != 0 && api.getNow() - old_Stoch[old_Stoch.Count - 1].timestamp > 1000 * 60 * 3)
                old_Stoch.RemoveAt(old_Stoch.Count - 1);

            if (old_Stoch.Count != 0)
            {
                double oldStochValue = old_Stoch[old_Stoch.Count - 1].value;
                double stochValue = stochTick.value;

                if (api.getShortPosition(instrument) == null && api.getLongPosition(instrument) == null && tradingTimeCode == 1)
                {
                    if (oldStochValue > threshold && stochValue < threshold && followTrend)
                        api.openShort(instrument);

                    if (oldStochValue < 1 - threshold && stochValue > 1 - threshold && followTrend)
                        api.openLong(instrument);

                    if (oldStochValue > 1 - threshold && stochValue < 1 - threshold && followTrend == false)
                        api.openShort(instrument);

                    if (oldStochValue < threshold && stochValue > threshold && followTrend == false)
                        api.openLong(instrument);
                }
            }

            if (api.getShortPosition(instrument) != null && api.getShortPosition(instrument).getDifference(nowTick) > takeprofit)
            {
                api.closeShort(instrument);
                hitTP++;
            }

            if (api.getLongPosition(instrument) != null && api.getLongPosition(instrument).getDifference(nowTick) > takeprofit)
            {
                api.closeLong(instrument);
                hitTP++;
            }

            if (api.getShortPosition(instrument) != null && api.getShortPosition(instrument).getDifference(nowTick) < -stoploss)
            {
                api.closeShort(instrument);
                hitSL++;
            }

            if (api.getLongPosition(instrument) != null && api.getLongPosition(instrument).getDifference(nowTick) < -stoploss)
            {
                api.closeLong(instrument);
                hitSL++;
            }

            old_Stoch.Insert(0, stochTick);
        }

        public override List<string> getUsedPairs()
        {
            return new List<string> { instrument };
        }
    }
}
