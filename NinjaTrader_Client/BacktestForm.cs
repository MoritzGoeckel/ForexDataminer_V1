using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client
{
    public partial class BacktestForm : Form
    {
        Database database;
        public BacktestForm(Database database, int backtestHours)
        {
            InitializeComponent();
            this.database = database;
            this.backtestHours = backtestHours;

            endTimestamp = database.getLastTimestamp();
            startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);
        }

        private Backtester backtester;

        private int backtestHours;
        private long endTimestamp, startTimestamp;

        private List<string> instruments = new List<string>();

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            instruments.Add("EURUSD");
            //instruments.Add("GBPUSD");
            //instruments.Add("USDJPY");
            //instruments.Add("USDCHF");

            backtester = new Backtester(database, 60, startTimestamp, endTimestamp, instruments);
            backtester.backtestResultArrived += backtester_backtestResultArrived;

            List<Strategy> strategies = new List<Strategy>();

            //Ergebnis:
            /*strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false)); //Gut
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0015, 0.0015, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0015, 0.0015, false)); //Gut
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0010, 0.0010, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0013, 0.0013, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0013, 0.0013, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false)); //Heigh risk
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false)); //Sehr gut, drawdown und profit gut

            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false)); //Heigh Frequency

            strategies.Add(new SSIStrategy(database));*/

            /*strategies.Add(new SSIStochStrategy(database, 0.002, 0.2, 1000 * 60 * 30, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.003, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6)); //Gut
            strategies.Add(new SSIStochStrategy(database, 0.001, 0.2, 1000 * 60 * 30, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.005, 0.2, 1000 * 60 * 40, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.004, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.005, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.004, 0.2, 1000 * 60 * 40, 1000 * 60 * 60 * 6));*/

            //strategies.Add(new SSIStochStrategy(database, 0.0, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6)); //Beste und allgemein


            //stretegies.Add(new SSI_Strategy(database));

            //New Strats here...


            /* HERE IS THE PLAYGROUND FOR TESTING */
            backtester.startBacktest(new SSIStochStrategy(database, 0.000, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6), instruments);

//            backtester.startBacktest(new SSIStrategy(database), instruments);
            
            //backtester.startBacktest(new SSIStochStrategy(database, 0.002, 0.2, 1000 * 60 * 60 * 3, 1000 * 60 * 60 * 5), instruments); //Allgemeiner
        }

        Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();

        void backtester_backtestResultArrived(Dictionary<string, BacktestResult> resultPerPair)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(resultPerPair)));
                return;
            }

            foreach(string instrument in instruments)
            {
                BacktestResult theResult = resultPerPair[instrument];

                int i = 1;
                string name = theResult.parameter["Strategy"] + "_" + theResult.parameter["Pair"];
                while (results.ContainsKey(name))
                {
                    name = theResult.parameter["Strategy"] + "_" + theResult.parameter["Pair"] + "_" + i;
                    i++;
                }

                results.Add(name, theResult);
                listBox_results.Items.Add(name);
            }
        }

        private void listBox_results_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            BacktestResult result = results[listBox_results.SelectedItem.ToString()];

            label_trades.Text = result.getTradesText();
            label_parameters.Text = result.getParameterText();
            label_result.Text = result.getResultText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BacktestResult result = results[listBox_results.SelectedItem.ToString()];
            ChartingForm chartingForm = new ChartingForm(database, result.getPositions(), startTimestamp, endTimestamp);
            chartingForm.Show(); //caching!
        }
    }
}
