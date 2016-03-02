using NinjaTrader_Client.Trader.Datamining;
using NinjaTrader_Client.Trader.Indicators;
using NinjaTrader_Client.Trader.MainAPIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader.Analysis
{
    public partial class DataminingForm : Form
    {
        DataminingDatabase dataminingDb;
        Database sourceDatabase;

        public DataminingForm(DataminingDatabase dataminingDb, Database sourceDatabase)
        {
            InitializeComponent();

            this.dataminingDb = dataminingDb;
            this.sourceDatabase = sourceDatabase;
        }

        private void DataminingForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataminingDb.importPair("EURUSD", sourceDatabase.getFirstTimestamp(), sourceDatabase.getLastTimestamp(), sourceDatabase);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dataminingDb.addOutcome(60 * 60 * 2);
            dataminingDb.addOutcome(60 * 60);
            dataminingDb.addOutcome(60 * 30);
            dataminingDb.addOutcome(60 * 10);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = dataminingDb.getProgress().getString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataminingDb.addData("ssi-mt4", sourceDatabase);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataminingDb.addData("ssi-win-mt", sourceDatabase);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //textBox1.Text = dataminingDb.getOutcomeIndicatorSampling(-1, 1, 60, "ssi-win-mt", 60 * 30, "EURUSD");
            //textBox1.Text = dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 60 * 30, "EURUSD");
            //textBox1.Text = dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 60 * 30, "EURUSD");
            //textBox1.Text += dataminingDb.getOutcomeIndicatorSampling(-1, 1, 50, "ssi-mt4", 60 * 60, "EURUSD");

            textBox1.Text += "MA1H-MA3H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA1-MA3", 60 * 10, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA3H-MA6H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA3-MA6", 60 * 10, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA9H-MA12H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA9-MA12", 60 * 10, "EURUSD") + Environment.NewLine;

            textBox1.Text += "MA1H-MA3H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA1-MA3", 60 * 30, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA3H-MA6H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA3-MA6", 60 * 30, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA9H-MA12H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA9-MA12", 60 * 30, "EURUSD") + Environment.NewLine;

            textBox1.Text += "MA1H-MA3H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA1-MA3", 60 * 60, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA3H-MA6H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA3-MA6", 60 * 60, "EURUSD") + Environment.NewLine;
            textBox1.Text += "MA9H-MA12H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-0.4, 0.4, 40, "MA9-MA12", 60 * 60, "EURUSD") + Environment.NewLine;

            textBox1.Text += "Stoch3H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 60 * 10, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch6H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 60 * 10, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch12H 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 60 * 10, "EURUSD") + Environment.NewLine;

            textBox1.Text += "Stoch3H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 60 * 30, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch6H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 60 * 30, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch12H 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 60 * 30, "EURUSD") + Environment.NewLine;

            textBox1.Text += "Stoch3H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 60 * 60, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch6H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 60 * 60, "EURUSD") + Environment.NewLine;
            textBox1.Text += "Stoch12H 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 60 * 60, "EURUSD") + Environment.NewLine;

            textBox1.Text += "SSI 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-mt4", 60 * 60, "EURUSD") + Environment.NewLine;
            textBox1.Text += "SSI-Win 60m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-win-mt", 60 * 60, "EURUSD") + Environment.NewLine;

            textBox1.Text += "SSI 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-mt4", 60 * 30, "EURUSD") + Environment.NewLine;
            textBox1.Text += "SSI-Win 30m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-win-mt", 60 * 30, "EURUSD") + Environment.NewLine;

            textBox1.Text += "SSI 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-mt4", 60 * 10, "EURUSD") + Environment.NewLine;
            textBox1.Text += "SSI-Win 10m Outcome" + Environment.NewLine + dataminingDb.getOutcomeIndicatorSampling(-1, 1, 20, "ssi-win-mt", 60 * 10, "EURUSD") + Environment.NewLine;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 1), "EURUSD", "last");
            dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 3), "EURUSD", "last");
            dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 6), "EURUSD", "last");
            dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 9), "EURUSD", "last");
            dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 12), "EURUSD", "last");

            //dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 1), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 3), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 6), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 12), "EURUSD", "last");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataminingDb.addMetaIndicatorDifference("MA_" + 1000 * 60 * 60 * 1 + "_last", "MA_" + 1000 * 60 * 60 * 3 + "_last", "MA1-MA3");
            dataminingDb.addMetaIndicatorDifference("MA_" + 1000 * 60 * 60 * 3 + "_last", "MA_" + 1000 * 60 * 60 * 6 + "_last", "MA3-MA6");
            dataminingDb.addMetaIndicatorDifference("MA_" + 1000 * 60 * 60 * 6 + "_last", "MA_" + 1000 * 60 * 60 * 9 + "_last", "MA6-MA9");
            dataminingDb.addMetaIndicatorDifference("MA_" + 1000 * 60 * 60 * 9 + "_last", "MA_" + 1000 * 60 * 60 * 12 + "_last", "MA9-MA12");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataminingDb.doMachineLearning(
                new string[] {
                    "MA1-MA3",
                    "MA3-MA6",
                    "MA9-MA12",
                    "Stoch_" + 1000 * 60 * 60 * 3 + "_last",
                    "Stoch_" + 1000 * 60 * 60 * 6 + "_last",
                    "Stoch_" + 1000 * 60 * 60 * 12 + "_last",
                    "ssi-mt4",
                    "ssi-win-mt"},
                "outcome_code_" + 60 * 30 + "_" + 0.10,
                "EURUSD", 
                Application.StartupPath + "/ann.NeuralNetwork");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //dataminingDb.addOutcomeCode(0.20, 60 * 30);
            //dataminingDb.addOutcomeCode(0.10, 60 * 10);
            //dataminingDb.addOutcomeCode(0.30, 60 * 60);
            dataminingDb.addOutcomeCode(0.10, 60 * 30);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Echt?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                dataminingDb.deleteAll();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Stoch12H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 98, 100, "EURUSD", 0.20, 0.20, false) + Environment.NewLine + Environment.NewLine;

            //Stoch6H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 96, 100, "EURUSD", 0.20, 0.20, false) + Environment.NewLine + Environment.NewLine;

            //Stoch3H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 98, 100, "EURUSD", 0.20, 0.20, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 52, 56, "EURUSD", 0.20, 0.20, true) + Environment.NewLine + Environment.NewLine;

            //MA9H-MA12H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.20, 0.20, false) + Environment.NewLine + Environment.NewLine;

            /*textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 98, 100, "EURUSD", 0.11, 0.11, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 98, 100, "EURUSD", 0.15, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 12 + "_last", 98, 100, "EURUSD", 0.20, 0.12, false) + Environment.NewLine + Environment.NewLine;

            //Stoch6H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 96, 100, "EURUSD", 0.10, 0.15, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 96, 100, "EURUSD", 0.11, 0.11, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 96, 100, "EURUSD", 0.15, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 96, 100, "EURUSD", 0.20, 0.12, false) + Environment.NewLine + Environment.NewLine;

            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 58, 62, "EURUSD", 0.11, 0.10, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 58, 62, "EURUSD", 0.11, 0.11, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 58, 62, "EURUSD", 0.15, 0.12, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 58, 62, "EURUSD", 0.20, 0.12, true) + Environment.NewLine + Environment.NewLine;

            //Stoch3H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 98, 100, "EURUSD", 0.10, 0.15, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 98, 100, "EURUSD", 0.11, 0.11, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 98, 100, "EURUSD", 0.15, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 98, 100, "EURUSD", 0.20, 0.12, false) + Environment.NewLine + Environment.NewLine;

            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 52, 56, "EURUSD", 0.11, 0.10, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 52, 56, "EURUSD", 0.11, 0.11, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 52, 56, "EURUSD", 0.15, 0.12, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "Stoch_" + 1000 * 60 * 60 * 3 + "_last", 52, 56, "EURUSD", 0.20, 0.12, true) + Environment.NewLine + Environment.NewLine;

            //Stoch6H 30m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 98, 100, "EURUSD", 0.10, 0.15, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 98, 100, "EURUSD", 0.11, 0.11, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 98, 100, "EURUSD", 0.15, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 98, 100, "EURUSD", 0.20, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 98, 100, "EURUSD", 0.10, 0.10, false) + Environment.NewLine + Environment.NewLine;

            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 56, 62, "EURUSD", 0.11, 0.10, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 56, 62, "EURUSD", 0.11, 0.11, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 56, 62, "EURUSD", 0.15, 0.12, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 56, 62, "EURUSD", 0.20, 0.12, true) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 30, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 56, 62, "EURUSD", 0.10, 0.10, true) + Environment.NewLine + Environment.NewLine;

            //MA9H-MA12H 60m Outcome
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.16, 0.16, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.16, 0.11, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.15, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.20, 0.12, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.10, 0.10, false) + Environment.NewLine + Environment.NewLine;
            textBox1.Text += dataminingDb.getSuccessRate(60 * 60, "MA9-MA12", 0.26, 0.32, "EURUSD", 0.12, 0.12, false) + Environment.NewLine + Environment.NewLine;*/

        }
    }
}
