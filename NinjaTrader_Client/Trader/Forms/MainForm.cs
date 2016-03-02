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
using System.Threading;

namespace NinjaTrader_Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Database database;
        private NinjaTraderAPI api;
        private SSI_Downloader ssi;

        private List<string> allInstruments = new List<string>();
        private List<string> minorsInstruments = new List<string>();
        private List<string> majorsInstruments = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            majorsInstruments.Add("EURUSD");
            majorsInstruments.Add("GBPUSD");
            majorsInstruments.Add("USDJPY");
            majorsInstruments.Add("USDCHF");

            minorsInstruments.Add("AUDCAD");
            minorsInstruments.Add("AUDJPY");
            minorsInstruments.Add("AUDUSD");
            minorsInstruments.Add("CHFJPY");
            minorsInstruments.Add("EURCHF");
            minorsInstruments.Add("EURGBP");
            minorsInstruments.Add("EURJPY");
            minorsInstruments.Add("GBPCHF");
            minorsInstruments.Add("GBPJPY");
            minorsInstruments.Add("NZDUSD");
            minorsInstruments.Add("USDCAD");

            allInstruments.AddRange(majorsInstruments);
            allInstruments.AddRange(minorsInstruments);

            Config.startConfig(Application.StartupPath);
            //mongodb = new MongoFacade(Config.mongodbExePath, Config.mongodbDataPath, "nt_trader");

            database = new SQLiteDatabase(Application.StartupPath + "//priceHistorySQLite.s3db");

            //database = new MongoDatabase(mongodb);
            api = new NinjaTraderAPI(allInstruments);
            ssi = new SSI_Downloader(allInstruments);

            NTLiveTradingAPI.createInstace(api, 125); //125 per position * 11 strategies = 1375 investement
        }

        private void ssi_sourceDataArrived(double value, long timestamp, string sourceName, string instrument)
        {
            database.setData(new TimeValueData(timestamp, value), sourceName, instrument);
        }

        int insertedSets = 0;
        private void api_tickdataArrived(Tickdata data, string instrument)
        {
            database.setPrice(data, instrument);

            this.Invoke(new Action(() => {
                    label1.Text = "Errors: " + 0 + Environment.NewLine +
                "Data collected: " + insertedSets++ + Environment.NewLine +
                Environment.NewLine +
                "Positionsize: " + NTLiveTradingAPI.getTheInstace().getPositionSize() + Environment.NewLine +
                "Cash value: " + NTLiveTradingAPI.getTheInstace().getCashValue() + Environment.NewLine +
                "Buying power: " + NTLiveTradingAPI.getTheInstace().getBuyingPower();
            }));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ssi.stop();
            api.stop();
            database.shutdown();
        }

        private bool isDownloadingUpdates = false;
        public void startDownloadingUpdates()
        {
            if (isDownloadingUpdates == false)
            {
                ssi.sourceDataArrived += ssi_sourceDataArrived;
                ssi.start();
                api.tickdataArrived += api_tickdataArrived;
                isDownloadingUpdates = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Start updating?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                button1.Enabled = false;
                startDownloadingUpdates();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartingForm cf = new ChartingForm(database, null, database.getLastTimestamp() - 1000 * 60 * 60, database.getLastTimestamp());
            cf.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExportImportForm eiForm = new ExportImportForm(database);
            eiForm.Show();
        }

        private void backtest_btn_Click(object sender, EventArgs e)
        {
            DedicatedStrategyBacktestForm backtestForm = new DedicatedStrategyBacktestForm(database);
            backtestForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LiveTradingForm form = new LiveTradingForm(NTLiveTradingAPI.getTheInstace());
            form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataDensityPerDayForm ddForm = new DataDensityPerDayForm(database);
            ddForm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RandomStrategyBacktestForm backtestForm = new RandomStrategyBacktestForm(database);
            backtestForm.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CorrelationAnalysisForm cf = new CorrelationAnalysisForm(database, 1000, 31, allInstruments);
            cf.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TradeHistoryChartForm thcf = new TradeHistoryChartForm();
            thcf.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DataDensityForm f = new DataDensityForm(database, 1000 * 60 * 60, "AUDUSD");
            f.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AnalyseRawTestDataForm form = new AnalyseRawTestDataForm();
            form.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Main: " + database.getSetsCount());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            double tradingTimeCode = new TradingTimeIndicator().getIndicator(Timestamp.getNow(), 0).value;
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
                database
            );
            df.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AutopilotTradingForm atf = new AutopilotTradingForm(database);
            atf.Show();
        }
    }
}
