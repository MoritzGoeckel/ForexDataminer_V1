﻿using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
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
            long currentTimestamp = startTimestamp;
            while (currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);

                foreach(string pair in pairs)
                    strat.doTick(pair);

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
