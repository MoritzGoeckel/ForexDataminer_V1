﻿using NinjaTrader_Client.Trader.BacktestBase;
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
            : base(database, 50 * 24, 10)
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

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref bool continueBacktesting)
        {
            instrument = all[z.Next(0, all.Count)];
            continueBacktesting = true;

            int r = z.Next(1, 5);

            /*if (r == 0)
                strategy = new BinaryStrategy(database,
                    sltp,
                    sltp,
                    generateInt(1000 * 60, 1000 * 60 * 60 * 5, 1000 * 60 * 5),
                    generateDouble(0.01, 1, 0.02),
                    generateBool());*/

            if (r == 1)
                strategy = new StochStrategy(database,
                    generateDouble(0.01, 1.5, 0.05),
                    generateDouble(0.01, 1.5, 0.05),
                    generateInt(1000 * 60 * 60 * 1, 1000 * 60 * 60 * 48, 1000 * 60 * 30),
                    generateDouble(0.00, 0.3, 0.02),
                    generateBool()
                    );

            else if(r == 2)
                strategy = new SSIStochStrategy(database,
                    generateDouble(0.01, 1.5, 0.05), //TP
                    generateDouble(0.01, 1.5, 0.05), //SL
                    generateDouble(0.05, 0.50, 0.01), //Threshold
                    generateInt(1000 * 60 * 30 * 1, 1000 * 60 * 30 * 8, 1000 * 60 * 5), //To
                    generateInt(1000 * 60 * 60 * 1, 1000 * 60 * 60 * 20, 1000 * 60 * 20), //StochTime
                    generateBool()); //againstCrowd

            else if (r == 3)
                strategy = new FastMovementStrategy(database,
                    generateInt(1000 * 60 * 1, 1000 * 60 * 30, 1000 * 60 * 5),
                    generateInt(1000 * 60 * 10 * 1, 1000 * 60 * 10 * 20, 1000 * 60 * 5),
                    generateDouble(0.01, 0.70, 0.03),
                    generateDouble(0.01, 1.5, 0.05),
                    generateDouble(0.01, 1.5, 0.05),
                    generateBool());

            else if (r == 4)
                strategy = new SSIStrategy(database,
                    generateDouble(0.0, 0.5, 0.01),
                    generateDouble(0.0, 0.5, 0.01),
                    generateBool());
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
