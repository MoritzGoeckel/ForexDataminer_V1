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
        protected string instrument;
        public Strategy(Database database, TradingAPI api, string instrument)
        {
            this.database = database;
            this.api = api;
            this.instrument = instrument;
        }

        public abstract void doTick();
        public abstract string getName();
    }
}
