using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
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
    public partial class DataDensityFrom : Form
    {
        Database database;
        Dictionary<string, DatacountHours> data = new Dictionary<string, DatacountHours>();

        struct DatacountHours
        {
            public int datacount, hours;
        }

        public DataDensityFrom(Database database)
        {
            this.database = database;
            InitializeComponent();
        }

        private void DataDensityFrom_Load(object sender, EventArgs e)
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
                int count = database.getPrices(currentTimestamp, currentTimestamp + 1000 * 60 * 60, "EURUSD").Count();
                DateTime time = new DateTime(1970, 1, 1);

                TimeSpan span = TimeSpan.FromMilliseconds(currentTimestamp);
                time = time.Add(span);

                if (count > 30)
                {
                    string key = time.ToString("{H}");

                    if (data.ContainsKey(key) == false)
                        data.Add(key, new DatacountHours());

                    DatacountHours thedata = data[key];
                    thedata.datacount += count;
                    thedata.hours++;

                    data[key] = thedata;
                }

                currentTimestamp += 1000 * 60 * 60;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.Text = "Hour\tData" + Environment.NewLine;
            foreach(KeyValuePair<string, DatacountHours> pair in data)
                textBox1.Text += pair.Key.Replace("{", "").Replace("}", "") + "\t" + (pair.Value.datacount / pair.Value.hours) + Environment.NewLine;
        }
    }
}
