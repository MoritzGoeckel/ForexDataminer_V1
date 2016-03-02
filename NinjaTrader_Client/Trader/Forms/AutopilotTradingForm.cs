using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client
{
    public partial class AutopilotTradingForm : Form
    {
        Database database;
        NinjaTraderAPI api;

        List<Strategy> runningStrategies = new List<Strategy>();

        private bool continueLiveTradingThread = true;
        private int tradingTick = 0;
        int strategyRunnerErrors = 0;
        long strategiesTickTime = -1;

        public AutopilotTradingForm(Database database)
        {
            this.database = database;
            this.api = NTLiveTradingAPI.getTheInstace().getAPI();
            api.tickdataArrived += AutopilotTradingForm_tickdataArrived;

            new Thread(delegate ()
            {
                while (continueLiveTradingThread)
                {
                    tradingTick++;
                    if (tradingTick > 1000)
                        tradingTick = 0;


                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    try
                    {
                        foreach (Strategy strat in runningStrategies)
                            try { strat.doTick(); }
                            catch { strategyRunnerErrors++; }
                    }
                    catch { }
                    sw.Stop();

                    strategiesTickTime = sw.ElapsedMilliseconds;

                    if(strategiesTickTime < 500)
                        Thread.Sleep(500 - Convert.ToInt32(strategiesTickTime));
                }
            }).Start();

            InitializeComponent();
        }

        private void AutopilotTradingForm_tickdataArrived(Tickdata data, string instrument)
        {
            label1.Text = "Connected: " + api.isConnected() + Environment.NewLine
                    + "Trading: " + continueLiveTradingThread + Environment.NewLine
                    + "Tick time: " + strategiesTickTime + Environment.NewLine;

            string strategieInfo = "";

            foreach (Strategy strat in runningStrategies)
            {
                strategieInfo += strat.getName() + " ";
                foreach (string pair in strat.getUsedPairs())
                    strategieInfo += pair;

                strategieInfo += Environment.NewLine;
            }

            /*if (instrument == "EURUSD")
            { //??? List Pairs
                label1.Text += "EURUSD" + Environment.NewLine 
                    + "Position: " + api.getMarketPosition("EURUSD") + Environment.NewLine
                    + "Profit: " + api.getProfit() + Environment.NewLine
                    + "Price: " + data.last + Environment.NewLine;
            }*/
        }

        public void startTradingLive(Strategy strat)
        {
            //Let the indicators catch up ???
            
            strat.setAPI(NTLiveTradingAPI.getTheInstace());
            runningStrategies.Add(strat);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //EURUSD Strat
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wirklich traden?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Config.startupPath;

                while (ofd.ShowDialog() != DialogResult.OK) ;

                List<string> stratStrings = BacktestFormatter.getParametersFromFile(ofd.FileName);

                foreach (string stratStr in stratStrings)
                {
                    Strategy strat = null;

                    BacktestFormatter.getStrategyFromString(database, stratStr, ref strat);
                    startTradingLive(strat);
                }

                button1.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            continueLiveTradingThread = false;
        }
    }
}
