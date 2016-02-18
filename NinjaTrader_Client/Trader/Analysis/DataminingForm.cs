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
            dataminingDb.addOutcome(60 * 60 * 2); //30 Minuten
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
            textBox1.Text = dataminingDb.getOutcomeIndicatorSampling(0, 100, 50, "Stoch_" + 1000 * 60 * 60 * 6 + "_last", 60 * 60 * 2, "EURUSD");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //dataminingDb.addIndicator(new MovingAverageIndicator(1000 * 60 * 60 * 6), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 3), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 6), "EURUSD", "last");
            dataminingDb.addIndicator(new StochIndicator(1000 * 60 * 60 * 12), "EURUSD", "last");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataminingDb.addMetaIndicatorDifference("MA_" + 1000 * 60 * 60 * 3 + "_last", "MA_" + 1000 * 60 * 60 * 6 + "_last", "MA3-MA6");
        }
    }
}
