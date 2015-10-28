using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtests
{
    class RandomStrategyBacktestForm : BacktestForm
    {
        public RandomStrategyBacktestForm(Database database)
            : base(database, 24 * 7, 60, true)
        { }

        protected override string getPairToTest()
        {
            return "EURUSD";
        }

        private Random z = new Random();
        protected override Strategy getRandomStrategyToTest()
        {
            //here is the place to generate random strategies
            return new SSIStochStrategy(database, Math.Round(z.NextDouble() * 0.5, 2) + 0.01, Math.Round(z.NextDouble() * 0.5, 2) + 0.01, Math.Round(z.NextDouble() * 0.15, 2) + 0.07, 1000 * 60 * 30 * z.Next(1, 8), 1000 * 60 * 60 * z.Next(1, 20));
            //return new FastMovement_Strategy(database, 1000 * 60 * z.Next(1, 30), 1000 * 60 * 10 * z.Next(1, 20), Math.Round(z.NextDouble() * 0.7, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, z.NextDouble() > 0.5);
        }

        protected override List<Strategy> getStrategiesToTest()
        {
            throw new NotImplementedException();
        }
    }
}
