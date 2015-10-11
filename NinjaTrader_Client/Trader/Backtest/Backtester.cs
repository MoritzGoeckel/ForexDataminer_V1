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

        public delegate void BacktestResultArrivedHandler(Dictionary<string, BacktestResult> result);
        public event BacktestResultArrivedHandler backtestResultArrived;

        private List<Thread> threads = new List<Thread>();
        private List<string> tradablePairs = new List<string>();

        public Backtester(Database database, int resolutionInSeconds, long startTimestamp, long endTimestamp, List<string> tradablePairs)
        {
            this.database = database;
            this.resolutionInSeconds = resolutionInSeconds;
            this.startTimestamp = startTimestamp;
            this.endTimestamp = endTimestamp;
            this.tradablePairs.AddRange(tradablePairs);
        }

        public void startBacktest(Strategy strat, string pair)
        {
            List<string> pairs = new List<string>();
            pairs.Add(pair);

            startBacktest(strat, pairs);
        }

        public void startBacktest(List<Strategy> strats, string pair)
        {
            List<string> pairs = new List<string>();
            pairs.Add(pair);

            foreach (Strategy strat in strats)
                startBacktest(strat, pairs);
        }

        public void startBacktest(Strategy strat, List<string> pairs)
        {
            BacktestTradingAPI dedicatedAPI = new BacktestTradingAPI(startTimestamp, database, tradablePairs);

            Strategy dedicatedStrategy = strat.copy();
            dedicatedStrategy.setAPI(dedicatedAPI); //Todo: Nicht schön, nicht sicher

            Thread thread = new Thread(() => runBacktest(dedicatedStrategy, pairs, dedicatedAPI));
            thread.Start();

            threads.Add(thread);
        }

        private void runBacktest(Strategy strat, List<string> pairs, BacktestTradingAPI api)
        {
            Stopwatch watch = new Stopwatch();
            long usedTime = 0;
            long doneTicks = 0;
            
            long currentTimestamp = startTimestamp;
            while (currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);

                foreach (string pair in pairs)
                {
                    watch.Reset();
                    watch.Start();
                    strat.doTick(pair);
                    watch.Stop();
                    usedTime += watch.ElapsedMilliseconds;
                    doneTicks++;
                }

                //Send these to the UI
                double timePerTick = (double)usedTime / (double)doneTicks;
                double done = (double)(currentTimestamp - startTimestamp) / (double)(endTimestamp - startTimestamp);
                double todo = (double)usedTime * (1d - done) / 1000d / 60d;

                string msg = timePerTick + " " + done + " " + todo;

                currentTimestamp += 1000 * resolutionInSeconds;
            }

            Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();
            foreach (string pair in pairs)
            {
                api.closePositions(pair);

                BacktestResult result = new BacktestResult(endTimestamp - startTimestamp, pair, strat.getName());
                result.addPositions(api.getHistory(pair));
                result = strat.addCustomVariables(result);

                results.Add(pair, result);

                if (backtestResultArrived != null)
                    backtestResultArrived(results);
            }
        }
    }
}
