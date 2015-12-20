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
    public partial class DataDensityForm : Form
    {
        Database database;
        Dictionary<string, int> data = new Dictionary<string, int>();

        int resolution;
        string pair;

        public DataDensityForm(Database database, int resolution, string pair)
        {
            this.database = database;
            this.resolution = resolution;
            this.pair = pair;

            InitializeComponent();
        }

        private void DataDensityForm_AllData_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            long endTimestamp = database.getLastTimestamp();
            long startTimestamp = database.getFirstTimestamp();

            long currentTimestamp = startTimestamp;

            while (currentTimestamp < endTimestamp)
            {
                int count = database.getPrices(currentTimestamp, currentTimestamp + resolution, pair).Count();
                DateTime time = new DateTime(1970, 1, 1);

                TimeSpan span = TimeSpan.FromMilliseconds(currentTimestamp);
                time = time.Add(span);

                string key = time.ToString("yyyy-MM-dd-HH");

                if (key.Contains("."))
                    throw new Exception("Why is that?");

                data.Add(key, count);

                currentTimestamp += resolution;

                backgroundWorker1.ReportProgress(Convert.ToInt32(((double)currentTimestamp - (double)startTimestamp) / ((double)endTimestamp - (double)startTimestamp) * 100));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Hour\tData" + Environment.NewLine);

            foreach (KeyValuePair<string, int> pair in data)
                builder.Append(pair.Key + "\t" + pair.Value + Environment.NewLine);

            textBox1.Text = builder.ToString();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Text = e.ProgressPercentage + "%";
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("Copied text to clipboard");
        }
    }
}
