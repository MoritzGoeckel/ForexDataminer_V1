using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.IO;
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
            : base(database, 90 * 24)
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
            while (true)
            {
                try
                {
                    File.AppendAllText(Config.startupPath + "/backtestes/raw_test_data.txt", BacktestFormatter.getDictStringCoded(result) + "@" + BacktestFormatter.getDictStringCoded(parameters) + Environment.NewLine);
                    break;
                }
                catch (Exception) { }
            }
        }

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref long resolutionInSeconds, ref bool continueBacktesting)
        {
            instrument = all[z.Next(0, all.Count)];
            continueBacktesting = true;

            resolutionInSeconds = 10; //Normalerweise 10er res, bei SSI 60er res

            int r = z.Next(1, 5);

            r = 4; //Nur SSI :) ???

            double sl = generateDouble(0.01, 0.3, 0.05);
            double tp = sl + generateDouble(0, 0.3, 0.05);

            if (r == 1)
                strategy = new StochStrategy(database,
                    sl,
                    tp,
                    generateInt(1000 * 60 * 30, 1000 * 60 * 60 * 12, 1000 * 60 * 30), //Stochtimeframe
                    generateDouble(0.00, 0.3, 0.05), //Threshold
                    generateBool()
                    );

            else if (r == 2)
            {
                strategy = new SSIStochStrategy(database,
                    tp, //TP
                    sl, //SL
                    generateDouble(0.00, 0.3, 0.05), //Threshold
                    generateInt(1000 * 60 * 30, 1000 * 60 * 60 * 10, 1000 * 60 * 30), //To
                    generateInt(1000 * 60 * 60 * 1, 1000 * 60 * 60 * 24, 1000 * 60 * 20), //StochTime
                    generateBool()); //againstCrowd

                instrument = majors[z.Next(0, majors.Count)];
            }

            else if (r == 3)
                strategy = new MomentumStrategy(database,
                    generateInt(1000 * 60 * 1, 1000 * 60 * 60, 1000 * 60 * 1), //Pre time
                    generateInt(1000 * 60 * 1, 1000 * 60 * 60, 1000 * 60 * 1), //Post time
                    generateDouble(0.01, 0.7, 0.01), //Threshold
                    tp,
                    sl,
                    generateBool());

            else if (r == 4)
            {
                double open = generateDouble(0.0, 0.6, 0.05);

                strategy = new SSIStrategy(database, 
                    open,  //Open
                    open - generateDouble(0.0, 0.8, 0.05), //Close
                    generateBool());

                resolutionInSeconds = 60;

                instrument = majors[z.Next(0, majors.Count)];
            }
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
