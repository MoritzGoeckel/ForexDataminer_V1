using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using NinjaTrader_Client.Trader.TradingAPIs;
using NinjaTrader_Client.Trader.Analysis;
using NinjaTrader_Client.Trader.Backtests;
using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Utils;
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
            NTLiveTradingAPI.createInstace(main.getAPI(), 125); //125 per position * 11 strategies = 1375 investement
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

            label1.Text = "Errors: " + data.dbErrors + Environment.NewLine +
                "Data collected: " + data.dataSets + Environment.NewLine + 
                "Tradingtick: " + data.tradingTick + Environment.NewLine +
                Environment.NewLine +
                "Positionsize: " + NTLiveTradingAPI.getTheInstace().getPositionSize() + Environment.NewLine +
                "Cash value: " + NTLiveTradingAPI.getTheInstace().getCashValue() + Environment.NewLine + 
                "Buying power: " + NTLiveTradingAPI.getTheInstace().getBuyingPower();

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
            cf.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wirklich migrieren?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                ((SQLiteDatabase)main.getDatabase()).migrate(new SQLDatabase());
                timer_migrate_update.Start();
            }
        }

        private void timer_migrate_update_Tick(object sender, EventArgs e)
        {
            label1.Text = "Migration:" + Environment.NewLine;
            try
            {
                foreach (KeyValuePair<string, double> pair in ((SQLiteDatabase)main.getDatabase()).migrateProgress)
                    label1.Text += pair.Key + ": " + pair.Value + Environment.NewLine;
            }
            catch (Exception) { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExportImportForm eiForm = new ExportImportForm(main.getDatabase());
            eiForm.Show();
        }

        private void backtest_btn_Click(object sender, EventArgs e)
        {
            DedicatedStrategyBacktestForm backtestForm = new DedicatedStrategyBacktestForm(main.getDatabase());
            backtestForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wirklich traden?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Config.startupPath;

                while (ofd.ShowDialog() != DialogResult.OK) ;

                List<string> strats = BacktestFormatter.getParametersFromFile(ofd.FileName);
                
                foreach (string stratStr in strats)
                {
                    Strategy strat = null;
                    string instrument = null;

                    BacktestFormatter.getStrategyFromString(main.getDatabase(), stratStr, ref strat, ref instrument);
                    main.startTradingLive(strat, instrument);
                }

                button5.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LiveTradingForm form = new LiveTradingForm(NTLiveTradingAPI.getTheInstace());
            form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataDensityPerDayForm ddForm = new DataDensityPerDayForm(main.getDatabase());
            ddForm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RandomStrategyBacktestForm backtestForm = new RandomStrategyBacktestForm(main.getDatabase());
            backtestForm.Show();
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
            cf.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TradeHistoryChartForm thcf = new TradeHistoryChartForm();
            thcf.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DataDensityForm f = new DataDensityForm(main.getDatabase(), 1000 * 60 * 60, "AUDUSD");
            f.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AnalyseRawTestDataForm form = new AnalyseRawTestDataForm();
            form.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Main: " + main.getDatabase().getSetsCount());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            double tradingTimeCode = new TradingTimeIndicator(main.getDatabase()).getIndicator(Timestamp.getNow(), "EURUSD").value;
            DateTime dt = Timestamp.getDate(Timestamp.getNow());
            MessageBox.Show(dt.ToString());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            StrategyStringGeneratorForm ssgf = new StrategyStringGeneratorForm();
            ssgf.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MongoFacade facade = new MongoFacade(Application.StartupPath + "\\MongoDB\\mongod.exe", Application.StartupPath + "\\MongoDB\\data", "dataminingDb");
            MongoDataminingDB dataminingDb = new MongoDataminingDB(facade);
            DataminingForm df = new DataminingForm(
                dataminingDb,
                main.getDatabase()
            );
            df.ShowDialog();
        }
    }
}
