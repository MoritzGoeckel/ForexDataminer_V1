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

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            int backtestHours = 48 * 3;

            long endTimestamp = database.getLastTimestamp();
            long startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);

            backtester = new Backtester(database, 20, startTimestamp, endTimestamp);
            backtester.backtestResultArrived += backtester_backtestResultArrived;
            
            //Do the backtest
            backtester.startBacktest(new FastMovement_Strategy(database), "EURUSD");
        }

        Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();

        void backtester_backtestResultArrived(BacktestResult result)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(result)));
                return;
            }

            string name = result.parameter["Strategy"] + "_" + result.parameter["Pair"];
            results.Add(name, result);
            listBox_results.Items.Add(name);
        }

        private void listBox_results_SelectedIndexChanged(object sender, EventArgs e)
        {
            BacktestResult result = results[listBox_results.SelectedItem.ToString()];

            label_trades.Text = result.getTradesText();
            label_parameters.Text = result.getParameterText();
            label_result.Text = result.getResultText();
        }
    }
}
