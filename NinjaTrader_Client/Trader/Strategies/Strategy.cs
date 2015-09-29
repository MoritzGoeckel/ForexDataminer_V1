using NinjaTrader_Client.Trader.Backtest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader
{
    public abstract class Strategy
    {
        protected Database database;
        protected TradingAPI api;
        public Strategy(Database database)
        {
            this.database = database;
        }

        public abstract void doTick();

        public abstract string getName();
        public abstract BacktestResult addCustomVariables(BacktestResult given);
        public abstract void resetStatistics();

        public void setAPI(TradingAPI api)
        {
            this.api = api;
        }
    }
}
