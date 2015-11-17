using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtest
{
    class Backtester
    {
        private Database database;
        private int resolutionInSeconds;
        private long startTimestamp, endTimestamp;

        public delegate void BacktestResultArrivedHandler(BacktestData result);
        public event BacktestResultArrivedHandler backtestResultArrived;

        private List<Thread> threads = new List<Thread>();
        public Dictionary<string, int> progress = new Dictionary<string, int>();

        private int maxPositionsPerHour;

        public Backtester(Database database, int resolutionInSeconds, long startTimestamp, long endTimestamp, int maxPositionsPerHour)
        {
            this.database = database;
            this.resolutionInSeconds = resolutionInSeconds;
            this.startTimestamp = startTimestamp;
            this.endTimestamp = endTimestamp;
            this.maxPositionsPerHour = maxPositionsPerHour;

            if (this.startTimestamp > this.endTimestamp)
                throw new Exception();
        }

        public void startBacktest(Strategy strat, string pair)
        {
            List<string> pairs = new List<string>();
            pairs.Add(pair);

            BacktestTradingAPI dedicatedAPI = new BacktestTradingAPI(startTimestamp, database, pairs);

            Strategy dedicatedStrategy = strat.copy();
            dedicatedStrategy.setAPI(dedicatedAPI); //Todo: Nicht schön, nicht sicher

            Thread thread = new Thread(() => runBacktest(dedicatedStrategy, pair, dedicatedAPI));
            thread.Start();

            threads.Add(thread);
        }

        public void startBacktest(List<Strategy> strats, string pair)
        {
            foreach (Strategy strat in strats)
                startBacktest(strat, pair);
        }

        long usedTime = 0;
        long doneTests = 0;

        private void runBacktest(Strategy strat, string pair, BacktestTradingAPI api)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            long currentTimestamp = startTimestamp;

            if (currentTimestamp > startTimestamp)
                throw new Exception();

            string name = getUniqueStrategyName(strat.getName());

            bool reportStrategy = true;
            while (currentTimestamp < endTimestamp)
            {
                long doneHours = (currentTimestamp - startTimestamp) / 1000 / 60 / 60;

                if (doneHours >= 1 && api.getHistory(pair).Count / doneHours > maxPositionsPerHour)
                {
                    reportStrategy = false;
                    break;
                }

                api.setNow(currentTimestamp);
                strat.doTick(pair);

                int percent = Convert.ToInt32(Convert.ToDouble(currentTimestamp - startTimestamp) / Convert.ToDouble(endTimestamp - startTimestamp) * 100);
                progress[name] = percent;

                currentTimestamp += 1000 * resolutionInSeconds;
            }

            progress.Remove(name);

            if (reportStrategy)
            {
                Dictionary<string, BacktestData> results = new Dictionary<string, BacktestData>();
                api.closePositions(pair);

                BacktestData result = new BacktestData(endTimestamp - startTimestamp, pair, strat.getName());
                result.setPositions(api.getHistory(pair));
                result.setParameter(strat.getParameters());
                result.setResult(strat.getResult());

                watch.Stop();
                usedTime += watch.ElapsedMilliseconds;
                doneTests++;

                timePerTest = (double)usedTime / (double)doneTests;

                if (backtestResultArrived != null)
                    backtestResultArrived(result);
            }
            else if (backtestResultArrived != null)
                backtestResultArrived(null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private string getUniqueStrategyName(string name)
        {
            int namePostfix = 0;
            while (progress.ContainsKey(name + "_" + namePostfix))
                namePostfix++;

            progress.Add(name + "_" + namePostfix, 0);

            return name + "_" + namePostfix;
        }

        public string getProgressText()
        {
            string output = "";

            try
            {
                foreach (KeyValuePair<string, int> pair in progress)
                    output += pair.Key + ": " + pair.Value + "%" + Environment.NewLine;
            }
            catch (Exception e) { }

            return output;
        }

        double timePerTest = 0;
        public double getAVGTimeTest()
        {
            return timePerTest;
        }

        public int getThreadsCount()
        {
            int i = 0;
            while (i < threads.Count)
            {
                if (threads[i].IsAlive)
                {
                    i++;
                }
                else
                    threads.RemoveAt(i);
            }

            return i;
        }
    }
}
