using NinjaTrader_Client.Trader.Model;
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

        public void startBacktest(List<Strategy> strats, List<string> pairs)
        {
            foreach (Strategy strat in strats)
                startBacktest(strat, pairs);
        }

        public void startBacktest(List<Strategy> strats, string pair)
        {
            foreach (Strategy strat in strats)
                startBacktest(strat, pair);
        }

        public void startBacktest(Strategy strat, List<string> pairs)
        {
            foreach (string pair in pairs)
                startBacktest(strat, pair);
        }

        public void startBacktest(Strategy strat, string pair)
        {
            BacktestTradingAPI api = new BacktestTradingAPI(startTimestamp, database, pair);
            strat.setAPI(api);

            Thread thread = new Thread(() => runBacktest(strat, api, pair));
            thread.Start();

            threads.Add(thread);
        }


        private void runBacktest(Strategy strat, BacktestTradingAPI api, string pair)
        {
            long currentTimestamp = startTimestamp;
            while (currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);

                if (api.isUptodate()) //Dataset not older than 3 minutes
                    strat.doTick();

                currentTimestamp += 1000 * resolutionInSeconds;
            }
            api.closePositions();

            BacktestResult result = new BacktestResult(endTimestamp - startTimestamp, pair, strat.getName());
            result.addPositions(api.getHistory());
            result = strat.addCustomVariables(result);

            if (backtestResultArrived != null)
                backtestResultArrived(result);
        }
    }
}
