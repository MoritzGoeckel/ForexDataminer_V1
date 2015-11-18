using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.Strategies;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Backtests
{
    class DedicatedStrategyBacktestForm : BacktestForm
    {
        List<string> parameterList = new List<string>();
        int nextStratId = 12;

        public DedicatedStrategyBacktestForm(Database database)
            : base(database, 38 * 24, 1)
        {
            //SSIStoch
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,24|threshold:0,18|to:5400000|stochT:54000000|sl:0,19|againstCrowd:True");
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,16|threshold:0,2|to:3600000|stochT:21600000|sl:0,15|againstCrowd:True");
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,12|threshold:0,2|to:5400000|stochT:7200000|sl:0,12|againstCrowd:True");

            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,14|threshold:0,15|to:10800000|stochT:14400000|sl:0,19|againstCrowd:False");
            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,23|threshold:0,22|to:9000000|stochT:14400000|sl:0,14|againstCrowd:False");
            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,29|threshold:0,19|to:9000000|stochT:3600000|sl:0,08|againstCrowd:True");

            parameterList.Add("pair:USDCHF|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,24|threshold:0,19|to:12600000|stochT:25200000|sl:0,2|againstCrowd:True");
            parameterList.Add("pair:USDCHF|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,26|threshold:0,09|to:5400000|stochT:10800000|sl:0,14|againstCrowd:True");
            parameterList.Add("pair:USDCHF|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,14|threshold:0,15|to:10800000|stochT:46800000|sl:0,29|againstCrowd:True");

            parameterList.Add("pair:USDJPY|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,13|threshold:0,09|to:5400000|stochT:3600000|sl:0,26|againstCrowd:False");
            parameterList.Add("pair:USDJPY|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,14|threshold:0,11|to:12600000|stochT:14400000|sl:0,29|againstCrowd:False");
            parameterList.Add("pair:USDJPY|timeframe:912|strategy:SSIStochStrategy_V0|tp:0,29|threshold:0,1|to:10800000|stochT:18000000|sl:0,25|againstCrowd:False");
        
            //SSI
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,31|thresholdClose:0,37|followTrend:False");
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0|thresholdClose:0,35|followTrend:False");

            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,21|thresholdClose:0,12|followTrend:True");
            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,17|thresholdClose:0,13|followTrend:True");

            parameterList.Add("pair:USDCHF|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,07|thresholdClose:0,34|followTrend:True");
            parameterList.Add("pair:USDCHF|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,15|thresholdClose:0,1|followTrend:True");

            parameterList.Add("pair:USDJPY|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,18|thresholdClose:0,1|followTrend:False");
            parameterList.Add("pair:USDJPY|timeframe:912|strategy:SSIStrategy_V0|thresholdOpen:0,23|thresholdClose:0,08|followTrend:False");

            //Fast movement
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:FastMovement-Strategy_V0|postT:1200000|preT:1140000|threshold:0,41|tp:0,28|sl:0,23|followTrend:True");
            parameterList.Add("pair:EURUSD|timeframe:912|strategy:FastMovement-Strategy_V0|postT:3600000|preT:960000|threshold:0,31|tp:0,21|sl:0,29|followTrend:True");

            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:FastMovement-Strategy_V0|postT:6600000|preT:540000|threshold:0,11|tp:0,23|sl:0,25|followTrend:True");
            parameterList.Add("pair:GBPUSD|timeframe:912|strategy:FastMovement-Strategy_V0|postT:6000000|preT:360000|threshold:0,14|tp:0,26|sl:0,15|followTrend:True");

            parameterList.Add("pair:USDCHF|timeframe:912|strategy:FastMovement-Strategy_V0|postT:11400000|preT:360000|threshold:0,15|tp:0,19|sl:0,2|followTrend:False");
            parameterList.Add("pair:USDCHF|timeframe:912|strategy:FastMovement-Strategy_V0|postT:4800000|preT:900000|threshold:0,02|tp:0,21|sl:0,07|followTrend:False");

            parameterList.Add("pair:USDJPY|timeframe:912|strategy:FastMovement-Strategy_V0|postT:8400000|preT:1740000|threshold:0,09|tp:0,24|sl:0,3|followTrend:False");
            parameterList.Add("pair:USDJPY|timeframe:912|strategy:FastMovement-Strategy_V0|postT:9000000|preT:1560000|threshold:0,1|tp:0,38|sl:0,24|followTrend:False");
            parameterList.Add("pair:USDJPY|timeframe:912|strategy:FastMovement-Strategy_V0|postT:10800000|preT:1440000|threshold:0,08|tp:0,18|sl:0,24|followTrend:False");

        }

        protected override void backtestResultArrived(Dictionary<string, string> parameters, Dictionary<string, string> result)
        {
            
        }

        protected override void getNextStrategyToTest(ref Strategy strategy, ref string instrument, ref bool continueBacktesting)
        {
            if (nextStratId < parameterList.Count)
                continueBacktesting = true;
            else
            {
                continueBacktesting = true;
                return;
            }

            BacktestFormatter.getStrategyFromString(database, parameterList[nextStratId], ref strategy, ref instrument);

            continueBacktesting = (strategy != null);

            nextStratId++;
        }
    }
}
