using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtests
{
    class DedicatedStrategyBacktestForm : BacktestForm
    {
        List<Dictionary<string, string>> parameterList = new List<Dictionary<string, string>>();
        int nextStratId = 0;

        public DedicatedStrategyBacktestForm(Database database)
            : base(database, 38 * 24, 10)
        {
            parameterList.Add(BacktestFormatter.convertStringCodedToParameters("pair:USDCHF|timeframe:912|strategy:SSIStochStrategy|tp:0,26|threshold:0,21|to:3600000|stochT:28800000|sl:0,2|againstCrowd:True"));
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

            strategy = null;
            if (parameterList[nextStratId]["strategy"] == "SSIStochStrategy")
                strategy = new SSIStochStrategy(database, parameterList[nextStratId]);

            instrument = parameterList[nextStratId]["pair"];

            continueBacktesting = (strategy != null);

            nextStratId++;
        }
    }
}
