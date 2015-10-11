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
            ChartingForm cf = new ChartingForm(main.getDatabase(), null, main.getDatabase().getLastTimestamp() - 1000 * 60 * 60 * 300, main.getDatabase().getLastTimestamp());
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
            BacktestForm backtestForm = new BacktestForm(main.getDatabase(), 24 * 16);
            backtestForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wirklich traden?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                List<string> tradablePairs = new List<string>();
                tradablePairs.Add("EURUSD");
                
                //Strategy strat = new FastMovement_Strategy(main.getDatabase(), 1000 * 60 * 3, 1000 * 60 * 13, 0.0008, 0.0013, 0.0013, false);
                Strategy strat = new SSIStochStrategy(main.getDatabase(), 0, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6); //tp 0.003
                
                strat.setAPI(new NTLiveTradingAPI(main.getAPI(), 100));
                main.startTradingLive(strat, tradablePairs);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NTLiveTradingAPI api = new NTLiveTradingAPI(main.getAPI(), 100);
            LiveTradingForm form = new LiveTradingForm(api);
            form.ShowDialog();
        }
    }
}
