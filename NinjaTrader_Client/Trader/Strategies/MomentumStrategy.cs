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
    public class MomentumStrategy : Strategy
    {
        int version = 0;

        private Indicator tradingTime;

        public MomentumStrategy(Database database, int preTime, int postTime, double thresholdPercent, double takeprofitPercent, double stoplossPercent, bool follow_trend)
            : base(database)
        {
            this.thresholdPercent = thresholdPercent;
            this.postTime = postTime;
            this.preTime = preTime;
            this.takeprofitPercent = takeprofitPercent;
            this.stoplossPercent = stoplossPercent;
            this.follow_trend = follow_trend;

            this.tradingTime = new TradingTimeIndicator(database);

            setupVisualizationData();
        }

        public MomentumStrategy(Database database, Dictionary<string, string> parameters) 
            : base(database)
        {
            preTime = Convert.ToInt32(parameters["preT"]);
            postTime = Convert.ToInt32(parameters["postT"]);
            thresholdPercent = Double.Parse(parameters["threshold"]);
            takeprofitPercent = Double.Parse(parameters["tp"]);
            stoplossPercent = Double.Parse(parameters["sl"]);
            follow_trend = Boolean.Parse(parameters["followTrend"]);

            setupVisualizationData();
        }

        public override Strategy copy()
        {
            return new MomentumStrategy(database, preTime, postTime, thresholdPercent, takeprofitPercent, stoplossPercent, follow_trend);
        }

        public override string getName()
        {
            return "MomentumStrategy" + "_V" + version;
        }

        public override Dictionary<string, string> getParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("postT", postTime.ToString());
            parameters.Add("preT", preTime.ToString());
            parameters.Add("threshold", thresholdPercent.ToString());
            parameters.Add("tp", takeprofitPercent.ToString());
            parameters.Add("sl", stoplossPercent.ToString());
            parameters.Add("followTrend", follow_trend.ToString());

            parameters.Add("name", getName());

            return parameters;
        }

        public override Dictionary<string, string> getResult()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("hitSl", hitSL.ToString());
            result.Add("hitTp", hitTP.ToString());
            result.Add("hitTo", hitTO.ToString());

            return result;
        }

        private BacktestVisualizationDataComponent price_vi, uptodate_vi, tradingTimeCode_vi, longDistance_vi, longThreshold_vi, shortDistance_vi, shortThreshold_vi, sl_vi, tp_vi;

        public override void setupVisualizationData()
        {
            price_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("price", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
            uptodate_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("uptodate", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 1));
            tradingTimeCode_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tradingTimeCode", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 2));

            longDistance_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("longDistance", BacktestVisualizationDataComponent.VisualizationType.OnChart, 3));
            longThreshold_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("longThreshold", BacktestVisualizationDataComponent.VisualizationType.OnChart, 3));

            shortDistance_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("longDistance", BacktestVisualizationDataComponent.VisualizationType.OnChart, 4));
            shortThreshold_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("shortThreshold", BacktestVisualizationDataComponent.VisualizationType.OnChart, 4));

            //sl_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("sl", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0)); //??? Wird nicht gesetzt ???
            //tp_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tp", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));
        }

        private List<Tickdata> old_tickdata = new List<Tickdata>();

        private double thresholdPercent;
        private int preTime, postTime;
        private double takeprofitPercent, stoplossPercent;
        private bool follow_trend;
        private int hitSL = 0, hitTP = 0, hitTO = 0;

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

            double threshold = api.getAvgPrice(instrument) * thresholdPercent / 100d;

            longThreshold_vi.value = threshold;
            shortThreshold_vi.value = threshold;

            double takeprofit = api.getAvgPrice(instrument) * takeprofitPercent / 100d;
            double stoploss = api.getAvgPrice(instrument) * stoplossPercent / 100d;

            //Remove old ones in the back
            //Now the tick old_tickdata[old_tickdata.Count - 1] is 5 or less minutes old
            while (old_tickdata.Count != 0 && api.getNow() - old_tickdata[old_tickdata.Count - 1].timestamp > preTime) //Warum sollte old_tickdata[old_tickdata.Count - 1] == null sein?
                old_tickdata.RemoveAt(old_tickdata.Count - 1);

            Tickdata nowTick = new Tickdata(api.getNow(), 0, api.getBid(instrument), api.getAsk(instrument));

            if (old_tickdata.Count != 0)
            {
                Tickdata pastTick = old_tickdata[old_tickdata.Count - 1]; //The tick in preTime
                bool longSignal = nowTick.bid - pastTick.ask > threshold;
                bool shortSignal = pastTick.bid - nowTick.ask > threshold;

                longDistance_vi.value = nowTick.bid - pastTick.ask;
                shortDistance_vi.value = pastTick.bid - nowTick.ask;

                if (follow_trend == false)
                {
                    bool oldLong = longSignal;
                    longSignal = shortSignal;
                    shortSignal = oldLong;

                    double oldLongDistance = longDistance_vi.value;
                    longDistance_vi.value = shortDistance_vi.value;
                    shortDistance_vi.value = oldLongDistance;
                }

                if (tradingTimeCode == 1)
                {
                    //New Orders if movement in preTime is > threshold
                    //Long
                    if (api.getLongPosition(instrument) == null && longSignal)
                        api.openLong(instrument);

                    //Short
                    if (api.getShortPosition(instrument) == null && shortSignal)
                        api.openShort(instrument);
                }

                //Close orders after postTime elapsed
                //Long
                if (api.getLongPosition(instrument) != null && api.getNow() - api.getLongPosition(instrument).timestampOpen > postTime && longSignal == false)
                {
                    api.closeLong(instrument);
                    hitTO++;
                }

                //Short
                if (api.getShortPosition(instrument) != null && api.getNow() - api.getShortPosition(instrument).timestampOpen > postTime && shortSignal == false)
                {
                    api.closeShort(instrument);
                    hitTO++;
                }

                //Close on win
                if (takeprofit != 0)
                {
                    if (api.getLongPosition(instrument) != null && api.getBid(instrument) > api.getLongPosition(instrument).priceOpen + takeprofit && longSignal == false)
                    {
                        api.closeLong(instrument);
                        hitTP++;
                    }

                    if (api.getShortPosition(instrument) != null && api.getAsk(instrument) + takeprofit < api.getShortPosition(instrument).priceOpen && shortSignal == false)
                    {
                        api.closeShort(instrument);
                        hitTP++;
                    }
                }

                //Close on loose
                if (stoploss != 0)
                {
                    if (api.getLongPosition(instrument) != null && api.getBid(instrument) + stoploss < api.getLongPosition(instrument).priceOpen && longSignal == false)
                    {
                        api.closeLong(instrument);
                        hitSL++;
                    }

                    if (api.getShortPosition(instrument) != null && api.getAsk(instrument) > api.getShortPosition(instrument).priceOpen + stoploss && shortSignal == false)
                    {
                        api.closeShort(instrument);
                        hitSL++;
                    }
                }
            }

            //add newest in front
            old_tickdata.Insert(0, nowTick);
        }
    }
}
