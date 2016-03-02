using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.BacktestBase.Visualization;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Strategies
{
    public abstract class Strategy
    {
        protected Database database;
        protected ITradingAPI api;
        public Strategy(Database database)
        {
            this.database = database;
        }

        public abstract void setupVisualizationData();

        protected BacktestVisualizationData visualizationData = new BacktestVisualizationData();
        public BacktestVisualizationData getVisualizationData()
        {
            return visualizationData.Copy();
        }

        public abstract void doTick();

        public abstract string getName();
        public abstract Dictionary<string, string> getParameters();
        public abstract Dictionary<string, string> getResult();

        public void setAPI(ITradingAPI api)
        {
            this.api = api;
        }

        public abstract Strategy copy();

        public abstract List<string> getUsedPairs();
    }
}
