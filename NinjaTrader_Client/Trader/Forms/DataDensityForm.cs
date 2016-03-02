using Microsoft.Office.Interop.Excel;
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

            /*Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            Workbook wb = excel.Workbooks.Add();
            Worksheet sh = wb.Sheets.Add();
            sh.Name = "TestSheet";
            sh.Cells[1, "A"].Value2 = "Date";
            sh.Cells[1, "B"].Value2 = "Count";

            int i = 1;
            foreach(KeyValuePair < string, int > pair in data)
            {
                i++;
                sh.Cells[i, "A"].Value2 = pair.Key.ToString();
                sh.Cells[i, "B"].Value2 = pair.Value.ToString();
            }

            var charts = sh.ChartObjects() as
                Microsoft.Office.Interop.Excel.ChartObjects;
            var chartObject = charts.Add(60, 10, 900, 300) as
                Microsoft.Office.Interop.Excel.ChartObject;
            var chart = chartObject.Chart;

            // Set chart range.
            var range = sh.get_Range(sh.Cells[1, "A"], sh.Cells[i, "B"]);
            chart.SetSourceData(range);

            // Set chart properties.
            chart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLine;

            chart.ChartWizard(Source: range,
                Title: "Data desity",
                CategoryTitle: "Date",
                ValueTitle: "Count");*/
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
