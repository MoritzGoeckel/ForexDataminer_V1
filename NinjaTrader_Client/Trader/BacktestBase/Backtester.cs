using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.BacktestBase.Visualization;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NinjaTrader_Client.Trader.Backtest
{
    class Backtester
    {
        private Database database;
        private long startTimestamp, endTimestamp;

        public delegate void BacktestResultArrivedHandler(BacktestData result);
        public event BacktestResultArrivedHandler backtestResultArrived;

        private List<Thread> threads = new List<Thread>();
        public Dictionary<string, ProgressReport> progress = new Dictionary<string, ProgressReport>();

        private int maxPositionsPerHour;

        public Backtester(Database database, long startTimestamp, long endTimestamp, int maxPositionsPerHour)
        {
            this.database = database;
            this.startTimestamp = startTimestamp;
            this.endTimestamp = endTimestamp;
            this.maxPositionsPerHour = maxPositionsPerHour;

            if (this.startTimestamp > this.endTimestamp)
                throw new Exception("BacktestConstructor: startTimestamp > endTimestamp");
        }

        public void startBacktest(Strategy strat, long resolutionInSeconds, int chartResolution)
        {
            BacktestTradingAPI dedicatedAPI = new BacktestTradingAPI(startTimestamp, database, strat.getUsedPairs());

            Strategy dedicatedStrategy = strat.copy();
            dedicatedStrategy.setAPI(dedicatedAPI); //Todo: Nicht schön, nicht sicher

            Thread thread = new Thread(() => runBacktest(dedicatedStrategy, resolutionInSeconds, chartResolution, dedicatedAPI));
            thread.Start();

            threads.Add(thread);
        }

        public void startBacktest(List<Strategy> strats, long resolutionInSeconds, int chartResolution)
        {
            foreach (Strategy strat in strats)
                startBacktest(strat, resolutionInSeconds, chartResolution);
        }

        double doneTestsTime = 0;
        double doneTestsCount = 0;

        double apiSetupCounts = 0;
        double apiSetupTime = 0;

        double strategyCalcCount = 0;
        double strategyCalcTime = 0;

        private void runBacktest(Strategy strat, long resolutionInSeconds, int visualizationSteps, BacktestTradingAPI api)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            long currentTimestamp = startTimestamp;

            if (currentTimestamp > startTimestamp)
                throw new Exception("Backtester runBacktest: currentTimestamp > startTimestamp");

            string name = getUniqueStrategyName(strat.getName());
            foreach (string pair in strat.getUsedPairs())
                name += " " + pair;

            List<KeyValuePair<long, BacktestVisualizationData>> visualizationData = new List<KeyValuePair<long, BacktestVisualizationData>>(visualizationSteps + 1);

            bool reportStrategy = true;
            try
            {
                double nextVisualizationRatio = 0.01;
                while (currentTimestamp < endTimestamp)
                {
                    long doneHours = (currentTimestamp - startTimestamp) / 1000 / 60 / 60;
                    double doneRatio = Convert.ToDouble(currentTimestamp - startTimestamp) / Convert.ToDouble(endTimestamp - startTimestamp);

                    if (doneHours >= 1 && api.getHistory(strat.getUsedPairs()[0]).Count / doneHours > maxPositionsPerHour) //strat.getNeededPairs()[0] Sollte eigentlich alle iterieren. ??? Todo (Performance?)
                    {
                        reportStrategy = false;
                        break;
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    api.setNow(currentTimestamp);
                    sw.Stop();

                    apiSetupCounts++;

                    double currentAPISetupTime = sw.Elapsed.TotalMilliseconds;
                    apiSetupTime += currentAPISetupTime;

                    sw.Reset();
                    sw.Start();
                    strat.doTick();
                    sw.Stop();

                    if (doneRatio >= nextVisualizationRatio)
                    {
                        visualizationData.Add(new KeyValuePair<long, BacktestVisualizationData>(currentTimestamp, strat.getVisualizationData()));
                        nextVisualizationRatio = doneRatio + (1 / visualizationSteps);
                    }

                    strategyCalcCount++;

                    double currentStrategyCalcTime = sw.Elapsed.TotalMilliseconds;
                    strategyCalcTime += currentStrategyCalcTime;

                    progress[name].apiSetupTime = currentAPISetupTime;
                    progress[name].strategyCalcTime = currentStrategyCalcTime;
                    progress[name].percentageDone = Convert.ToInt32(doneRatio * 100);

                    currentTimestamp += 1000 * resolutionInSeconds;
                }
            }
            catch (Exception e)
            {
                reportStrategy = false;

                bool done = false;
                while (done == false)
                { 
                    try
                    {
                        File.AppendAllText(Config.errorLogPath, "Backtest:" + "\t" + e.Source + "->" + e.Message + "\t" + BacktestFormatter.getDictStringCoded(strat.getParameters()) + Environment.NewLine + "Trace: " + e.StackTrace + Environment.NewLine);
                        done = true;
                    }
                    catch (Exception) { Thread.Sleep(100); }
                }
            }

            progress.Remove(name);

            if (reportStrategy)
            {
                Dictionary<string, BacktestData> results = new Dictionary<string, BacktestData>();
                foreach(string pair in strat.getUsedPairs())
                    api.closePositions(pair);

                //Result temporaily for only the first pair ??? todo: Multiple results ???
                string resultInstrument = strat.getUsedPairs()[0];

                BacktestData result = new BacktestData(endTimestamp - startTimestamp, resultInstrument, strat.getName());
                result.setPositions(api.getHistory(resultInstrument));
                result.setParameter(strat.getParameters());
                result.setResult(strat.getResult());
                result.setVisualizationData(visualizationData);

                watch.Stop();
                doneTestsTime += watch.Elapsed.TotalMilliseconds;
                doneTestsCount++;

                if (backtestResultArrived != null)
                    backtestResultArrived(result);
            }
            else if (backtestResultArrived != null)
                backtestResultArrived(null);
        }

        internal double getMillisecondsStrategyTimePerTick()
        {
            return strategyCalcTime / strategyCalcCount;
        }

        internal double getMillisecondsAPITimePerTick()
        {
            return apiSetupTime / apiSetupCounts;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private string getUniqueStrategyName(string name)
        {
            int namePostfix = 0;
            while (progress.ContainsKey(name + "_" + namePostfix))
                namePostfix++;

            progress.Add(name + "_" + namePostfix, new ProgressReport());

            return name + "_" + namePostfix;
        }

        public string getProgressText()
        {
            string output = "";

            try
            {
                foreach (KeyValuePair<string, ProgressReport> pair in progress)
                    output += pair.Key + ": " + pair.Value.percentageDone + "% | API: " + Math.Round(pair.Value.apiSetupTime, 4) + "ms | Strat: " + Math.Round(pair.Value.strategyCalcTime, 4) + "ms" + Environment.NewLine;
            }
            catch (Exception) { }

            return output;
        }

        public double getMillisecondsPerTest()
        {
            return doneTestsTime / doneTestsCount;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
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
