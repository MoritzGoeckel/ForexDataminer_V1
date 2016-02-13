using NinjaTrader_Client.Trader.BacktestBase.Visualization;
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

        private WalkerIndicator tradingTime;

        public SSIStrategy(Database database, double thresholdOpen, double thresholdClose, bool followCrowd)
            : base(database)
        {
            this.thresholdOpen = thresholdOpen;
            this.thresholdClose = thresholdClose;
            this.followCrowd = followCrowd;

            this.tradingTime = new TradingTimeIndicator();

            if (thresholdClose >= thresholdOpen)
                throw new Exception("thresholdClose has to be < thresholdOpen");

            setupVisualizationData();
        }

        public SSIStrategy(Database database, Dictionary<string, string> parameters) : base(database)
        {
            thresholdOpen = Double.Parse(parameters["thresholdOpen"]);
            thresholdClose = Double.Parse(parameters["thresholdClose"]);
            followCrowd = Boolean.Parse(parameters["followCrowd"]);
            
            if (thresholdClose >= thresholdOpen)
                throw new Exception("thresholdClose has to be < thresholdOpen");

            setupVisualizationData();
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

        private BacktestVisualizationDataComponent price_vi, uptodate_vi, tradingTimeCode_vi, ssi_vi;

        public override void setupVisualizationData()
        {
            price_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("price", BacktestVisualizationDataComponent.VisualizationType.OnChart, 0));

            visualizationData.addComponent(new BacktestVisualizationDataComponent("thresholdClose", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1, thresholdClose));
            visualizationData.addComponent(new BacktestVisualizationDataComponent("thresholdOpen", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1,  thresholdOpen));

            visualizationData.addComponent(new BacktestVisualizationDataComponent("thresholdClose_Lower", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1,-thresholdClose));
            visualizationData.addComponent(new BacktestVisualizationDataComponent("thresholdOpen_Lower", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1, -thresholdOpen));

            visualizationData.addComponent(new BacktestVisualizationDataComponent("NULL", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1, 0));

            uptodate_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("uptodate", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 2));
            tradingTimeCode_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("tradingTimeCode", BacktestVisualizationDataComponent.VisualizationType.TrafficLight, 3));
            ssi_vi = visualizationData.addComponent(new BacktestVisualizationDataComponent("ssi", BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne, 1));
        }

        public override void doTick(string instrument)
        {
            price_vi.value = api.getAvgPrice(instrument);

            bool isUpToDate = api.isUptodate(instrument);
            uptodate_vi.value = isUpToDate ? 1 : 0;
            if (isUpToDate == false)
                return;

            double tradingTimeCode = tradingTime.getIndicator(api.getNow(), 0).value;
            tradingTimeCode_vi.value = tradingTimeCode;

            if (tradingTimeCode == 0)
            {
                //Flatten everything
                api.closePositions(instrument);
                return;
            }

            TimeValueData ssiValue = database.getData(api.getNow(), "ssi-mt4", instrument);
            double ssi = ssiValue.value;
            ssi_vi.value = ssi;

            if (ssiValue == null)
                return;

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
