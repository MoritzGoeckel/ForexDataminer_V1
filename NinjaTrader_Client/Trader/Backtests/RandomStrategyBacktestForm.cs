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
        private List<string> majors = new List<string>();

        public RandomStrategyBacktestForm(Database database)
            : base(database, 38 * 24, 10, true)
        {
            majors.Add("EURUSD");
            majors.Add("GBPUSD");
            majors.Add("USDJPY");
            majors.Add("USDCHF");
        }

        protected override string getPairToTest()
        {
            return majors[z.Next(0, majors.Count)];
        }

        private Random z = new Random();
        protected override Strategy getRandomStrategyToTest()
        {
            //here is the place to generate random strategies

            int r = z.Next(0, 6);

            if (r <= 2)
                return new SSIStochStrategy(database, Math.Round(z.NextDouble() * 0.3, 2) + 0.01, Math.Round(z.NextDouble() * 0.3, 2) + 0.01, Math.Round(z.NextDouble() * 0.15, 2) + 0.07, 1000 * 60 * 30 * z.Next(1, 8), 1000 * 60 * 60 * z.Next(1, 20), true);
            else if (r <= 4)
                return new FastMovement_Strategy(database, 1000 * 60 * z.Next(1, 30), 1000 * 60 * 10 * z.Next(1, 20), Math.Round(z.NextDouble() * 0.7, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, z.NextDouble() > 0.5);
            else
                return new SSIStrategy(database, Math.Round(z.NextDouble() * 0.5, 2), Math.Round(z.NextDouble() * 0.5, 2), z.NextDouble() > 0.5);
        }

        protected override List<Strategy> getStrategiesToTest()
        {
            throw new NotImplementedException();
        }
    }
}
