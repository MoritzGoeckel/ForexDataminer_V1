using ChartDirector;
using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader;
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

            this.Controls.Add(upperChart);
            
            //Create the chart
            SimpleChart sc = new SimpleChart(database);

            if (historicPositions != null)
                sc.addHistoricPositions(historicPositions);

            sc.drawPriceChartWithSSI(upperChart, this.Width, this.Height, "EURUSD", timestampStart, timestampEnd);
        }
    }
}
