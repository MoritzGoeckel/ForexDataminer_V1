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
            : base(database, 38 * 24, 10)
        {
            majors.Add("EURUSD");
            majors.Add("GBPUSD");
            majors.Add("USDJPY");
            majors.Add("USDCHF");
        }

        private Random z = new Random();
        protected override void backtestResultArrived(Dictionary<string, string> parameters, Dictionary<string, string> result)
        {
            
        }

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref bool continueBacktesting)
        {
            instrument = majors[z.Next(0, majors.Count)];
            continueBacktesting = true;

            int r = z.Next(0, 6);

            if (r <= 2)
                strategy = new SSIStochStrategy(database, Math.Round(z.NextDouble() * 0.3, 2) + 0.01, Math.Round(z.NextDouble() * 0.3, 2) + 0.01, Math.Round(z.NextDouble() * 0.15, 2) + 0.07, 1000 * 60 * 30 * z.Next(1, 8), 1000 * 60 * 60 * z.Next(1, 20), true);
            else if (r <= 4)
                strategy = new FastMovement_Strategy(database, 1000 * 60 * z.Next(1, 30), 1000 * 60 * 10 * z.Next(1, 20), Math.Round(z.NextDouble() * 0.7, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, z.NextDouble() > 0.5);
            else
                strategy = new SSIStrategy(database, Math.Round(z.NextDouble() * 0.5, 2), Math.Round(z.NextDouble() * 0.5, 2), z.NextDouble() > 0.5);
        }
    }
}
