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
    public partial class BacktestForm : Form
    {
        Database database;
        public BacktestForm(Database database, int backtestHours, bool doRandomTests)
        {
            InitializeComponent();
            this.database = database;
            this.backtestHours = backtestHours;

            endTimestamp = database.getLastTimestamp();
            startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);
            this.doRandomTests = doRandomTests;
        }

        private Backtester backtester;

        private int backtestHours;
        private long endTimestamp, startTimestamp;

        private bool doRandomTests;

        private List<string> instruments = new List<string>();

        private void BacktestForm_Load(object sender, EventArgs e)
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

            //Hier verwendete Pairs angeben
            //instruments.AddRange(majors);

            instruments.Add("EURUSD");

            backtester = new Backtester(database, 60, startTimestamp, endTimestamp);
            backtester.backtestResultArrived += backtester_backtestResultArrived;

            if (doRandomTests)
            {
                backtester.backtestResultArrived += startNewBacktests;
                startNewBacktests(null);
                startNewBacktests(null);
                startNewBacktests(null);
                startNewBacktests(null);
            }
            else
            {
                List<Strategy> strats = new List<Strategy>();
                strats.Add(new SSIStochStrategy(database, 0.29, 0.49, 0.16, 1000 * 60 * 150, 1000 * 60 * 1080));
                strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.16, 1000 * 60 * 159, 1000 * 60 * 1032));
                strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.16, 1000 * 60 * 159, 1000 * 60 * 1032));
                strats.Add(new SSIStochStrategy(database, 0.35, 0.3, 0.16, 1000 * 60 * 159, 1000 * 60 * 1032));

                backtester.startBacktest(strats, instruments[0]);
            }

            //strats.Add(new SSIStochStrategy(database, 0.15, 0.15, 0.2, 1000 * 60 * 60 * 2, 1000 * 60 * 60 * 4));

            //strats.Add(new SSIStochStrategy(database, 0.10, 0.10, 0.15, 1000 * 60 * 60 * 2, 1000 * 60 * 60 * 5));
            //strats.Add(new SSIStochStrategy(database, 0.10, 0.10, 0.2, 1000 * 60 * 60 * 2, 1000 * 60 * 60 * 5));

            

            //strats.Add(new SSIStrategy(database, 0.2, 0.2));
            //strats.Add(new SSIStrategy(database, 0.1, 0.1));
            //strats.Add(new SSIStrategy(database, 0.1, 0.2));
            //strats.Add(new SSIStrategy(database, 0.2, 0.1));

            /*strats.Add(new SSIStochStrategy(database, 0.02, 0.10, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 3));
            strats.Add(new SSIStochStrategy(database, 0.02, 0.10, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 4));
            strats.Add(new SSIStochStrategy(database, 0.02, 0.10, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 5));
            strats.Add(new SSIStochStrategy(database, 0.02, 0.10, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6));
            strats.Add(new SSIStochStrategy(database, 0.02, 0.10, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 7));*/


            //backtester.startBacktest(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 15, 0.60, 0.01, 0.02, true), instruments);

            /*
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.08, 0.15, 0.15, false)); //Gut
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.12, 0.15, 0.15, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.10, 0.15, 0.15, false)); //Gut
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.10, 0.10, 0.10, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.12, 0.10, 0.10, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.12, 0.13, 0.13, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 1, 1000 * 60 * 13, 0.10, 0.13, 0.13, false));
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.08, 0.15, 0.15, false)); //Heigh risk
            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 2, 1000 * 60 * 13, 0.08, 0.13, 0.13, false)); //Sehr gut, drawdown und profit gut

            strategies.Add(new FastMovement_Strategy(database, 1000 * 60 * 3, 1000 * 60 * 13, 0.08, 0.13, 0.13, false)); //Heigh Frequency

            strategies.Add(new SSIStrategy(database));*/

            /*strategies.Add(new SSIStochStrategy(database, 0.2, 0.2, 1000 * 60 * 30, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.3, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6)); //Gut
            strategies.Add(new SSIStochStrategy(database, 0.1, 0.2, 1000 * 60 * 30, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.5, 0.2, 1000 * 60 * 40, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.4, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.5, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6));
            strategies.Add(new SSIStochStrategy(database, 0.4, 0.2, 1000 * 60 * 40, 1000 * 60 * 60 * 6));*/

            //strategies.Add(new SSIStochStrategy(database, 0.0, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6)); //Beste und allgemein

            //stretegies.Add(new SSI_Strategy(database));

            //strategies.Add(new SSIStochStrategy(database, 0.03, 0.2, 1000 * 60 * 30, 1000 * 60 * 60 * 6));
            //strategies.Add(new SSIStochStrategy(database, 0.03, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 5));

            //strategies.Add(new SSIStochStrategy(database, 0.03, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6));
            //strategies.Add(new SSIStochStrategy(database, 0.04, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6));
            //strategies.Add(new SSIStochStrategy(database, 0.05, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6));
            //strategies.Add(new SSIStochStrategy(database, 0.06, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6));

            //strategies.Add(new SSIStochStrategy(database, 0, 0.2, 1000 * 60 * 20, 1000 * 60 * 60 * 6));

            //backtester.startBacktest(strategies, "EURUSD");

            //backtester.startBacktest(new SSIStochStrategy(database, 0.05, 0.2, 1000 * 60 * 60, 1000 * 60 * 60 * 6), instruments);
            
            //backtester.startBacktest(new SSIStochStrategy(database, 0.2, 0.2, 1000 * 60 * 60 * 3, 1000 * 60 * 60 * 5), instruments); //Allgemeiner
        }

        private Random z = new Random();
        private int tested = 0;

        void startNewBacktests(Dictionary<string, BacktestResult> resultPerPair)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => startNewBacktests(resultPerPair)));
                return;
            }

            //backtester.startBacktest(new SSIStochStrategy(database, Math.Round(z.NextDouble() * 0.5, 2) + 0.01, Math.Round(z.NextDouble() * 0.5, 2) + 0.01, Math.Round(z.NextDouble() * 0.15, 2) + 0.07, 1000 * 60 * 30 * z.Next(1, 8), 1000 * 60 * 60 * z.Next(1, 20)), "EURUSD");
            backtester.startBacktest(new FastMovement_Strategy(database, 1000 * 60 * z.Next(1, 30), 1000 * 60 * 10 * z.Next(1, 20), Math.Round(z.NextDouble() * 0.7, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, Math.Round(z.NextDouble() * 0.4, 2) + 0.01, z.NextDouble() > 0.5), "EURUSD");
            
            this.Text = "Random tests: " + tested++.ToString();
        }

        Dictionary<string, BacktestResult> results = new Dictionary<string, BacktestResult>();

        void backtester_backtestResultArrived(Dictionary<string, BacktestResult> resultPerPair)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => backtester_backtestResultArrived(resultPerPair)));
                return;
            }

            //Output to interface
            foreach(string instrument in instruments)
            {
                BacktestResult theResult = resultPerPair[instrument];

                double score = Convert.ToDouble(theResult.getResult("Profit")) - Convert.ToDouble(theResult.getResult("Positions")) * 0.0001d;

                int i = 1;
                string name = Math.Round(score, 4) + theResult.parameter["Strategy"] + "_" + theResult.parameter["Pair"];
                while (results.ContainsKey(name))
                {
                    name = Math.Round(score, 4) + " " + theResult.parameter["Strategy"] + "_" + theResult.parameter["Pair"] + "_" + i;
                    i++;
                }

                results.Add(name, theResult);
                listBox_results.Items.Add(name);

                //Output to file
                if (Directory.Exists(Application.StartupPath + "/backtestes/") == false)
                    Directory.CreateDirectory(Application.StartupPath + "/backtestes/");

                string path = Application.StartupPath + "/backtestes/" + instrument + "_" + DateTime.Now.ToString("yyyy-M-d") +".csv";

                if (File.Exists(path) == false)
                    File.WriteAllText(path, "Score;" + theResult.getCSVHeader() + Environment.NewLine);
                File.AppendAllText(path, score + ";" + theResult.getCSV() + Environment.NewLine);
            }
        }

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
