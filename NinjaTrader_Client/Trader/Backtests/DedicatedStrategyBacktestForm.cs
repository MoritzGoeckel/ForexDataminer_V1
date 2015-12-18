using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader.Backtests
{
    class DedicatedStrategyBacktestForm : BacktestForm
    {
        List<string> parameterList = new List<string>();
        int nextStratId = 0;

        public DedicatedStrategyBacktestForm(Database database)
            : base(database, 9 * 24, 10)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Config.startupPath;

            while (ofd.ShowDialog() != DialogResult.OK);

            parameterList = BacktestFormatter.getParametersFromFile(ofd.FileName);
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
