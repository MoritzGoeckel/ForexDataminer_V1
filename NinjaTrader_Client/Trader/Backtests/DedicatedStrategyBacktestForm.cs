using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtests
{
    class DedicatedStrategyBacktestForm : BacktestForm
    {
        public DedicatedStrategyBacktestForm(Database database)
            : base(database, 24 * 7, 60, false)
        { }

        protected override string getPairToTest()
        {
            return "EURUSD";
        }

        protected override Strategy getRandomStrategyToTest()
        {
            throw new NotImplementedException();
        }

        protected override List<Strategy> getStrategiesToTest()
        {
            List<Strategy> strats = new List<Strategy>();

            //here is the place to insert strategies for testing
            strats.Add(new SSIStochStrategy(database, 0.29, 0.49, 0.16, 1000 * 60 * 150, 1000 * 60 * 1080));
            strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.16, 1000 * 60 * 159, 1000 * 60 * 1032));

            return strats;
        }
    }
}
