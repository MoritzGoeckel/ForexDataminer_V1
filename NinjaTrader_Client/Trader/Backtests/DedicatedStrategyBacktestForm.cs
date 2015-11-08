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
            : base(database, 31 * 24, 60, false)
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

            //EURUSD
            //strats.Add(new SSIStochStrategy(database, 0.29, 0.49, 0.16, 1000 * 60 * 150, 1000 * 60 * 1080)); //#1
            //strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.16, 1000 * 60 * 159, 1000 * 60 * 1032)); //avg besser

            //USDJPY
            //strats.Add(new SSIStochStrategy(database, 0.2, 0.11, 0.08, 120 * 60 * 1000, 60 * 60 * 1000)); //#1 besser
            //strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.1, 175 * 60 * 1000, 830 * 60 * 1000)); //avg
            //strats.Add(new SSIStochStrategy(database, 0.31, 0.1, 0.08, 180 * 60 * 1000, 540 * 60 * 1000)); //better?            

            strats.Add(new SSIStrategy(database, 0.2, 0.2, false));
            strats.Add(new SSIStrategy(database, 0.1, 0.2, false));
            strats.Add(new SSIStrategy(database, 0.2, 0.1, false)); // Beste
            strats.Add(new SSIStrategy(database, 0.1, 0.1, false));

            strats.Add(new SSIStrategy(database, 0.2, 0.2, true));
            strats.Add(new SSIStrategy(database, 0.1, 0.2, true));
            strats.Add(new SSIStrategy(database, 0.2, 0.1, true));
            strats.Add(new SSIStrategy(database, 0.1, 0.1, true));

            return strats;
        }
    }
}
