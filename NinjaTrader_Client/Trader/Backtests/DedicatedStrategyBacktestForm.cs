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
        List<Strategy> strats = new List<Strategy>();
        int nextStratId = 0;

        public DedicatedStrategyBacktestForm(Database database)
            : base(database, 31 * 24, 60)
        {
            strats.Add(new SSIStrategy(database, 0.2, 0.2, false));
            strats.Add(new SSIStrategy(database, 0.1, 0.2, false));
            strats.Add(new SSIStrategy(database, 0.2, 0.1, false));
            strats.Add(new SSIStrategy(database, 0.1, 0.1, false));

            strats.Add(new SSIStrategy(database, 0.2, 0.2, true));
            strats.Add(new SSIStrategy(database, 0.1, 0.2, true));
            strats.Add(new SSIStrategy(database, 0.2, 0.1, true));
            strats.Add(new SSIStrategy(database, 0.1, 0.1, true));
        }

        protected override void backtestResultArrived(Dictionary<string, string> parameters, Dictionary<string, string> result)
        {
            
        }

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref bool continueBacktesting)
        {
            if (nextStratId < strats.Count)
                continueBacktesting = true;
            else
            {
                continueBacktesting = true;
                return;
            }

            strategy = strats[nextStratId];
            nextStratId++;

            instrument = "EURUSD";
        }
    }
}
