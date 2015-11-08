using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NinjaTrader.Client;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using NinjaTrader_Client.Trader.Indicators;
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
            NTLiveTradingAPI.createInstace(main.getAPI(), 500); //500 per position * 2 strategies = 1000 investement
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
            label3.Text = "Tradingtick: " + data.tradingTick;
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
                Strategy usdStrat = new SSIStochStrategy(main.getDatabase(), 0.09, 0.28, 0.17, 1000 * 60 * 90, 1000 * 60 * 120); //#1
                main.startTradingLive(usdStrat, "EURUSD");
                
                //JPYUSD
                Strategy jpyStrat = new SSIStrategy(main.getDatabase(), 0.1, 0.04, false); //#1
                main.startTradingLive(jpyStrat, "JPYUSD");

                //GBPUSD
                Strategy gbpStrat = new SSIStrategy(main.getDatabase(), 0.19, 0.12, true); //#1
                main.startTradingLive(jpyStrat, "GBPUSD");

                //USDCHF
                Strategy chfStrat = new SSIStochStrategy(main.getDatabase(), 0.23, 0.25, 0.14, 180 * 60 * 1000, 420 * 60 * 1000); //#1
                main.startTradingLive(jpyStrat, "USDCHF");

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
    }
}
