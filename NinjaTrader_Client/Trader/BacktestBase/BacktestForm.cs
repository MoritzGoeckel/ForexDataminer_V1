using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Backtest;
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
        public BacktestForm(Database database, int backtestHours, int resolution, bool doRandomTests)
        {
            InitializeComponent();
            this.database = database;
            this.backtestHours = backtestHours;

            endTimestamp = database.getLastTimestamp();
            startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);
            this.doRandomTests = doRandomTests;
            this.resolution = resolution;
        }

        protected abstract List<Strategy> getStrategiesToTest();
        protected abstract Strategy getRandomStrategyToTest();
        protected abstract string getPairToTest();

        private Backtester backtester;
        protected Database database;

        private int backtestHours;
        private long endTimestamp, startTimestamp;
        private int resolution;
        private bool doRandomTests;

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            backtester = new Backtester(database, resolution, startTimestamp, endTimestamp);
            backtester.backtestResultArrived += backtester_backtestResultArrived;

            if (doRandomTests)
            {
                backtester.backtestResultArrived += startNewRandomBacktest;
                startNewRandomBacktest(null);
                startNewRandomBacktest(null);
                startNewRandomBacktest(null);
                startNewRandomBacktest(null);
            }
            else
                backtester.startBacktest(getStrategiesToTest(), getPairToTest());
        }

        private int testsCount = 0;

        private void startNewRandomBacktest(BacktestResult result)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => startNewRandomBacktest(result)));
                return;
            }

            backtester.startBacktest(getRandomStrategyToTest(), getPairToTest());
        }

        private Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();

        private void backtester_backtestResultArrived(BacktestResult result)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(result)));
                return;
            }

            double score = Convert.ToDouble(result.getResult("Profit")) - Convert.ToDouble(result.getResult("Positions")) * 0.0001d;

            //Output to interface
            int i = 1;
            string name = Math.Round(score, 4) + result.parameter["Strategy"] + "_" + result.parameter["Pair"];
            while (results.ContainsKey(name))
            {
                name = Math.Round(score, 4) + " " + result.parameter["Strategy"] + "_" + result.parameter["Pair"] + "_" + i;
                i++;
            }

            results.Add(name, result);
            listBox_results.Items.Add(name);

            int threads = backtester.getThreadsCount();
            this.Text = "Tests: " + (++testsCount).ToString() + " | Sec/Test: " + Math.Round(backtester.getAVGTimeTest() / 1000, 4) + " | Threads: " + threads + " | = " + Math.Round(backtester.getAVGTimeTest() / 1000 / threads) + " Seconds";

            //Output to file
            if (Directory.Exists(Application.StartupPath + "/backtestes/") == false)
                Directory.CreateDirectory(Application.StartupPath + "/backtestes/");

            string path = Application.StartupPath + "/backtestes/" + result.parameter["Pair"] + "_" + result.parameter["Strategy"] + "_" + DateTime.Now.ToString("yyyy-M-d") + ".csv";

            if (File.Exists(path) == false)
                File.WriteAllText(path, "Score;" + result.getCSVHeader() + Environment.NewLine);
            File.AppendAllText(path, score + ";" + result.getCSV() + Environment.NewLine);
        }

        //UI stuff
        private void listBox_results_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox_results.SelectedItem != null)
            {
                BacktestResult result = results[listBox_results.SelectedItem.ToString()];

                label_trades.Text = result.getTradesText();
                label_parameters.Text = result.getParameterText();
                label_result.Text = result.getResultText();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BacktestResult result = results[listBox_results.SelectedItem.ToString()];
            ChartingForm chartingForm = new ChartingForm(database, result.getPositions(), startTimestamp, endTimestamp);
            chartingForm.Show(); //caching!
        }
    }
}
