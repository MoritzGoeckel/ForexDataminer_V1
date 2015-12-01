using NinjaTrader_Client.Trader.MainAPIs;
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
        private List<string> minors = new List<string>();
        private List<string> all = new List<string>();

        public RandomStrategyBacktestForm(Database database)
            : base(database, 38 * 24, 10)
        {
            majors.Add("EURUSD");
            majors.Add("GBPUSD");
            majors.Add("USDJPY");
            majors.Add("USDCHF");

            minors.Add("AUDCAD");
            minors.Add("AUDJPY");
            minors.Add("AUDUSD");
            minors.Add("CHFJPY");
            minors.Add("EURCHF");
            minors.Add("EURGBP");
            minors.Add("EURJPY");
            minors.Add("GBPCHF");
            minors.Add("GBPJPY");
            minors.Add("NZDUSD");
            minors.Add("USDCAD");

            all.AddRange(majors);
            all.AddRange(minors);
        }

        private Random z = new Random();
        protected override void backtestResultArrived(Dictionary<string, string> parameters, Dictionary<string, string> result)
        {
            /*double profit = double.Parse(result["profit"]);
            string pair = result["pair"];
            int positions = Convert.ToInt32(result["positions"]);

            if(positions < 1000)
            {
                profit stark im minus und strategie umkehrbar -> kehre um und starte
            }*/
        }

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref bool continueBacktesting)
        {
            instrument = all[z.Next(0, all.Count)];
            continueBacktesting = true;

            strategy = new BinaryStrategy(database,
                generateDouble(0.01,0.5, 0.01),
                generateDouble(0.01, 0.5, 0.01),
                generateInt(1000 * 60, 1000 * 60 * 60 * 5, 1000 * 60 * 5),
                generateDouble(0.01, 1, 0.02),
                generateBool());

            /*int r = z.Next(0, 6);

            if (r <= 2)
                strategy = new SSIStochStrategy(database,
                    generateDouble(0.01, 0.50, 0.01), //TP
                    generateDouble(0.01, 0.50, 0.01), //SL
                    generateDouble(0.05, 0.50, 0.01), //Threshold
                    generateInt(1000 * 60 * 30 * 1, 1000 * 60 * 30 * 8, 1000 * 60 * 5), //To
                    generateInt(1000 * 60 * 60 * 1, 1000 * 60 * 60 * 20, 1000 * 60 * 20), //StochTime
                    generateBool()); //againstCrowd

            else if (r <= 4)
                strategy = new FastMovementStrategy(database,
                    generateInt(1000 * 60 * 1, 1000 * 60 * 30, 1000 * 60 * 5),
                    generateInt(1000 * 60 * 10 * 1, 1000 * 60 * 10 * 20, 1000 * 60 * 5),
                    generateDouble(0.01, 0.70, 0.03),
                    generateDouble(0.01, 0.40, 0.02),
                    generateDouble(0.01, 0.40, 0.02),
                    generateBool());

            else
                strategy = new SSIStrategy(database,
                    generateDouble(0.0, 0.5, 0.01),
                    generateDouble(0.0, 0.5, 0.01),
                    generateBool());*/
        }

        private bool generateBool()
        {
            return z.NextDouble() > 0.5;
        }

        private double generateDouble(double min, double max, double stepSize)
        {
            int stepCount = Convert.ToInt32(Math.Round((max - min) / Convert.ToDouble(stepSize)));
            return Math.Round(min + (z.Next(0, stepCount) * stepSize), 2);
        }

        private int generateInt(int min, int max, int stepSize)
        {
            int stepCount = Convert.ToInt32(Math.Round(Convert.ToDouble(max - min) / Convert.ToDouble(stepSize)));
            return min + (z.Next(0, stepCount) * stepSize);
        }
    }
}
