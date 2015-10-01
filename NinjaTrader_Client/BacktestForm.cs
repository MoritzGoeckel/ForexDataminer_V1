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
        public BacktestForm(Database database)
        {
            InitializeComponent();
            this.database = database;
        }

        Backtester backtester;

        int backtestHours = 48 * 3;
        long endTimestamp, startTimestamp;

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            endTimestamp = database.getLastTimestamp();
            startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);

            backtester = new Backtester(database, 1, startTimestamp, endTimestamp); //20
            backtester.backtestResultArrived += backtester_backtestResultArrived;

            List<string> instruments = new List<string>();
            instruments.Add("EURUSD");
            instruments.Add("GBPUSD");
            instruments.Add("USDJPY");
            instruments.Add("USDCHF");

            List<Strategy> stretegies = new List<Strategy>();

            //Good strats für 20er auflösung:
            /*stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 7, 1000 * 60 * 60 * 1, 0.0017, 0.0000, 0.0023));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 5, 1000 * 60 * 20, 0.0020, 0.0015, 0.0015));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 20, 0.0012, 0.0012, 0.0015));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 130, 1000 * 60 * 5, 0.0010, 0.0010, 0.0015));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 10, 0.0010, 0.0005, 0.0015)); //Good*/

            //Positiv im 1er resolution
            /*stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 10, 0.0010, 0.0007, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 10, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 10, 0.0010, 0.0007, 0.0010, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 5, 0.0010, 0.0007, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 5, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 5, 0.0010, 0.0007, 0.0010, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0007, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0005, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false)); //Beste
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0007, 0.0010, false));


            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 15, 0.0010, 0.0007, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 15, 0.0010, 0.0005, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 15, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 15, 0.0010, 0.0007, 0.0010, false));


            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 17, 0.0010, 0.0007, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 17, 0.0010, 0.0005, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 17, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 17, 0.0010, 0.0007, 0.0010, false));*/

            //Runde 2
            /*stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0010, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0010, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0010, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0010, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0010, 0.0015, 0.0015, false));


            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0012, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0012, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0012, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0012, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0012, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0012, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0012, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0012, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0012, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0012, 0.0015, 0.0015, false));


            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0008, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0008, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0008, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false));

            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0005, 0.0005, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0007, 0.0007, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false));*/

            //Ergebnis:
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false)); //Gut
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0015, 0.0015, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0015, 0.0015, false)); //Gut
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0010, 0.0010, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0012, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.0010, 0.0013, 0.0013, false));
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false)); //Sehr gut, drawdown und profit gut
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0015, 0.0015, false)); //Heigh risk
            stretegies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false)); //Heigh Frequency

            //stretegies.Add(new SSI_Strategy(database));

            //New Strats here...


            /* HERE IS THE PLAYGROUND FOR TESTING */

            backtester.startBacktest(stretegies, "EURUSD");
        }

        Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();

        Random z = new Random();
        void backtester_backtestResultArrived(BacktestResult result)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(result)));
                return;
            }

            int i = 1;
            string name = result.parameter["Strategy"] + "_" + result.parameter["Pair"];

            while (results.ContainsKey(name))
            {
                name = result.parameter["Strategy"] + "_" + result.parameter["Pair"] + "_" + i;
                i++;
            }

            results.Add(name, result);
            listBox_results.Items.Add(name);
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
            chartingForm.Show(); //Baue canching!
        }
    }
}
