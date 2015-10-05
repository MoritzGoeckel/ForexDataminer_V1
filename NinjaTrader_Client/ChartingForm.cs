using ChartDirector;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Charting;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client
{
    public partial class ChartingForm : Form
    {
        Database database;
        private List<TradePosition> historicPositions;

        private long timestampStart, timestampEnd;

        public ChartingForm(Database database, List<TradePosition> historicPositions, long timestampStart, long timestampEnd)
        {
            InitializeComponent();
            this.database = database;
            this.historicPositions = historicPositions;
            this.timestampEnd = timestampEnd;
            this.timestampStart = timestampStart;
        }

        WinChartViewer upperChart;
        private void ChartingForm_Load(object sender, EventArgs e)
        {
            upperChart = new WinChartViewer();
            upperChart.BackColor = Color.Black;
            upperChart.Dock = DockStyle.Fill;
            upperChart.DoubleClick += upperChart_DoubleClick;

            this.Controls.Add(upperChart);
            
            //Create the chart
            SimpleChart sc = new SimpleChart(database);

            if (historicPositions != null)
                sc.addHistoricPositions(historicPositions);

            sc.drawPriceChartWithSSI(upperChart, this.Width, this.Height, "EURUSD", timestampStart, timestampEnd);
        }

        void upperChart_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.InitialDirectory = Application.StartupPath;
            if(fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                upperChart.Image.Save(fd.FileName);
        }
    }
}
