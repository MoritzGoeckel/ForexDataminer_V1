using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public delegate void BacktestResultArrivedHandler(BacktestResult result);
        public event BacktestResultArrivedHandler backtestResultArrived;

        private List<Thread> threads = new List<Thread>();

        public Backtester(Database database, int resolutionInSeconds, long startTimestamp, long endTimestamp)
        {
            this.database = database;
            this.resolutionInSeconds = resolutionInSeconds;
            this.startTimestamp = startTimestamp;
            this.endTimestamp = endTimestamp;
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
            while (currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);
                strat.doTick(pair);
                
                currentTimestamp += 1000 * resolutionInSeconds;
            }

            Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();
            api.closePositions(pair);

            BacktestResult result = new BacktestResult(endTimestamp - startTimestamp, pair, strat.getName());
            result.addPositions(api.getHistory(pair));
            result = strat.addCustomVariables(result);

            watch.Stop();
            usedTime += watch.ElapsedMilliseconds;
            doneTests++;

            timePerTest = (double)usedTime / (double)doneTests;

            if (backtestResultArrived != null)
                backtestResultArrived(result);
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
