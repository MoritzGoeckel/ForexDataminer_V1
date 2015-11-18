using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using NinjaTrader_Client.Trader.Analysis;
using NinjaTrader_Client.Trader.Backtests;

namespace NinjaTrader_Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Main main;
        private void Form1_Load(object sender, EventArgs e)
        {
            main = new Main(Application.StartupPath);
            main.uiDataChanged += updateUI;
            NTLiveTradingAPI.createInstace(main.getAPI(), 125); //125 per position * 4 strategies = 500 investement
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.stop();
        }

        public void updateUI(UIData data)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => updateUI(data)));
                return;
            }

            label1.Text = "Errors: " + data.dbErrors;
            label2.Text = "Datasets: " + data.dataSets;
            label3.Text = "Tradingtick: " + data.tradingTick + Environment.NewLine +
                "Positionsize: " + NTLiveTradingAPI.getTheInstace().getPositionSize() + Environment.NewLine +
                "CV: " + NTLiveTradingAPI.getTheInstace().getCashValue() + " BP: " + NTLiveTradingAPI.getTheInstace().getBuyingPower();

            //setPositionSize(Convert.ToInt32(getCashValue() / 4 / 4));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Start updating?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                button1.Enabled = false;
                main.startDownloadingUpdates();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartingForm cf = new ChartingForm(main.getDatabase(), null, main.getDatabase().getLastTimestamp() - 1000 * 60 * 60, main.getDatabase().getLastTimestamp());
            cf.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Wirklich migrieren?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                main.getDatabase().megrate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExportImportForm eiForm = new ExportImportForm(main.getDatabase());
            eiForm.ShowDialog();
        }

        private void backtest_btn_Click(object sender, EventArgs e)
        {
            DedicatedStrategyBacktestForm backtestForm = new DedicatedStrategyBacktestForm(main.getDatabase());
            backtestForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wirklich traden?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {                
                //EURUSD
                Strategy usdStrat = new SSIStochStrategy(main.getDatabase(), 0.23, 0.23, 0.9, 1000 * 60 * 90, 1000 * 60 * 120, true);
                main.startTradingLive(usdStrat, "EURUSD");

                //USDJPY
                Strategy jpyStrat = new FastMovement_Strategy(main.getDatabase(), 18 * 60 * 1000, 160 * 60 * 1000, 0.3, 0.25, 0.17, false);
                main.startTradingLive(jpyStrat, "USDJPY");

                //GBPUSD
                Strategy gbpStrat = new SSIStrategy(main.getDatabase(), 0.19, 0.12, true);
                main.startTradingLive(gbpStrat, "GBPUSD");

                //USDCHF
                Strategy chfStrat = new SSIStochStrategy(main.getDatabase(), 0.24, 0.24, 0.14, 180 * 60 * 1000, 420 * 60 * 1000, true);
                main.startTradingLive(chfStrat, "USDCHF");

                button5.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LiveTradingForm form = new LiveTradingForm(NTLiveTradingAPI.getTheInstace());
            form.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataDensityFrom ddForm = new DataDensityFrom(main.getDatabase());
            ddForm.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RandomStrategyBacktestForm backtestForm = new RandomStrategyBacktestForm(main.getDatabase());
            backtestForm.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<string> majors = new List<string>();
            majors.Add("EURUSD");
            majors.Add("GBPUSD");
            majors.Add("USDJPY");
            majors.Add("USDCHF");

            List<string> minors = new List<string>();
            minors.Add("AUDCAD");
            minors.Add("AUDJPY");
            minors.Add("AUDUSD");
            minors.Add("CHFJPY");
            minors.Add("EURCHF");
            minors.Add("EURGBP");
            minors.Add("EURJPY");
            minors.Add("GBPCHF");
            minors.Add("GBPJPY");
            minors.Add("NZDUSD");
            minors.Add("USDCAD");

            List<string> all = new List<string>();
            all.AddRange(majors);
            all.AddRange(minors);

            CorrelationAnalysisForm cf = new CorrelationAnalysisForm(main.getDatabase(), 1000, 31, all);
            cf.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TradeHistoryChartForm thcf = new TradeHistoryChartForm();
            thcf.ShowDialog();
        }
    }
}
