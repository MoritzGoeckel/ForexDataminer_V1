using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client
{
    public abstract partial class BacktestForm : Form
    {
        protected abstract void getNextStrategyToTest(ref Strategy strategy, ref String instrument, ref bool continueBacktesting);
        protected abstract void backtestResultArrived(Dictionary<string, string> parameters, Dictionary<string, string> result);

        private Backtester backtester;
        protected Database database;

        private long endTimestamp, startTimestamp;
        private int resolution;

        private int testsCount = 0;
        private Dictionary<string, BacktestData> results = new Dictionary<string, BacktestData>();

        public BacktestForm(Database database, int backtestHours, int resolution)
        {
            InitializeComponent();
            this.database = database;

            this.endTimestamp = database.getLastTimestamp();
            this.startTimestamp = endTimestamp - (backtestHours * 60L * 60L * 1000L);
            this.resolution = resolution;
        }

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            backtester = new Backtester(database, resolution, startTimestamp, endTimestamp);
            backtester.backtestResultArrived += backtester_backtestResultArrived;

            startNewBacktests();
        }

        private void startNewBacktests()
        {
            int maxThreads = Environment.ProcessorCount + (Environment.ProcessorCount / 4);
            while (backtester.getThreadsCount() < maxThreads)
            {
                bool continueTesting = false;
                string pair = null;
                Strategy strategy = null;
                
                getNextStrategyToTest(ref strategy, ref pair, ref continueTesting);

                if (continueTesting && pair != null && strategy != null)
                    backtester.startBacktest(strategy, pair);
                else
                    break;
            }

            outputThreadCount();
        }

        private void backtester_backtestResultArrived(BacktestData result)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(result)));
                return;
            }

            outputThreadCount();

            //Output to interface
            int i = 1;
            string name = result.getParameters()["strategy"] + "_" + result.getParameters()["pair"];
            while (results.ContainsKey(name))
            {
                name = result.getParameters()["strategy"] + "_" + result.getParameters()["pair"] + "_" + i;
                i++;
            }

            results.Add(name, result);
            listBox_results.Items.Add(name);

            int threads = backtester.getThreadsCount();
            this.Text = "Tests: " + (++testsCount).ToString() + " | Sec/Test: " + Math.Round(backtester.getAVGTimeTest() / 1000, 4) + " | Threads: " + threads + " | = " + Math.Round(backtester.getAVGTimeTest() / 1000 / threads) + " Seconds";

            //Output to file
            if (Directory.Exists(Application.StartupPath + "/backtestes/") == false)
                Directory.CreateDirectory(Application.StartupPath + "/backtestes/");

            string path = Application.StartupPath + "/backtestes/backtest-" + result.getParameters()["strategy"] + ".csv";

            if (File.Exists(path) == false)
                File.WriteAllText(path, BacktestFormatter.getCSVHeader(result) + Environment.NewLine);
            File.AppendAllText(path, BacktestFormatter.getCSVLine(result) + Environment.NewLine);

            this.backtestResultArrived(result.getParameters(), result.getResult());
            startNewBacktests();
        }

        private void outputThreadCount()
        {
            threadsLabel.Text = "Threads: " + backtester.getThreadsCount();
        }

        //UI stuff
        private void listBox_results_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox_results.SelectedItem != null)
            {
                BacktestData result = results[listBox_results.SelectedItem.ToString()];

                label_trades.Text = BacktestFormatter.getPositionsText(result);
                label_parameters.Text = BacktestFormatter.getParameterText(result);
                label_result.Text = BacktestFormatter.getResultText(result);
            }
        }

        private void openChartBtn_Click(object sender, EventArgs e)
        {
            BacktestData result = results[listBox_results.SelectedItem.ToString()];
            ChartingForm chartingForm = new ChartingForm(database, result.getPositions(), startTimestamp, endTimestamp);
            chartingForm.Show(); //caching! ???
        }
    }
}