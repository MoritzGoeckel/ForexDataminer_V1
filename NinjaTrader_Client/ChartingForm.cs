using ChartDirector;
using NinjaTrader_Client.Model;
using NinjaTrader_Client.Trader;
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
        public ChartingForm(Database database)
        {
            InitializeComponent();
            this.database = database;
        }

        WinChartViewer upperChart;
        private void ChartingForm_Load(object sender, EventArgs e)
        {
            upperChart = new WinChartViewer();
            upperChart.BackColor = Color.Black;
            upperChart.Dock = DockStyle.Fill;

            this.Controls.Add(upperChart);

            drawPriceChart(upperChart, "EURUSD", database.getLastTimestamp() - 1000 * 60 * 60 * 48, Timestamp.getNow());
        }

        private void drawPriceChart(WinChartViewer viewer, string instrument, long start, long end)
        {
            XYChart c = new XYChart(this.Width, this.Height);
            c.setColor(Chart.TextColor, HexColorCodes.white);
            c.setColor(Chart.BackgroundColor, HexColorCodes.black);

            // Add a title box using grey (0x555555) 20pt Arial font
            c.addTitle("Prices", "Arial", 20, HexColorCodes.white);

            // Set the plotarea at (70, 70) and of size 500 x 300 pixels, with transparent
            // background and border and light grey (0xcccccc) horizontal grid lines
            c.setPlotArea(70, 50, this.Width - (2 * 70), this.Height - 70, Chart.Transparent, -1, Chart.Transparent, HexColorCodes.black);

            // Add a legend box with horizontal layout above the plot area at (70, 35). Use 12pt
            // Arial font, transparent background and border, and line style legend icon.
            LegendBox b = c.addLegend(10, 10, false, "Arial", 12);
            b.setBackground(Chart.Transparent, Chart.Transparent);
            b.setLineStyleKey();

            // Set axis label font to 12pt Arial
            c.xAxis().setLabelStyle("Arial", 12);
            c.yAxis().setLabelStyle("Arial", 12);
            c.yAxis2().setLabelStyle("Arial", 12);
            c.yAxis2().setColors(Chart.TextColor, HexColorCodes.strongGreen);

            // Set the x and y axis stems to transparent, and the x-axis tick color to grey
            // (0xaaaaaa)
            c.xAxis().setColors(Chart.Transparent, Chart.TextColor, Chart.TextColor, HexColorCodes.white);
            c.yAxis().setColors(Chart.Transparent);
            c.yAxis2().setColors(Chart.Transparent);

            // Set the major/minor tick lengths for the x-axis to 10 and 0.
            c.xAxis().setTickLength(10, 0);

            // For the automatic axis labels, set the minimum spacing to 80/40 pixels for the x/y
            // axis.
            c.xAxis().setTickDensity(80);
            c.yAxis().setTickDensity(40);
            c.yAxis2().setTickDensity(40);

            StepLineLayer price_layer = c.addStepLineLayer(); //addLineLayer2
            StepLineLayer oszilator_layer = c.addStepLineLayer();
            oszilator_layer.setUseYAxis2();

            price_layer.setLineWidth(1);
            oszilator_layer.setLineWidth(2);

            oszilator_layer.setDataCombineMethod(Chart.Side);

            //Get the data database ???
            List<Tickdata> ticks = database.getPrices(start, end, instrument);

            double[] asks = new double[ticks.Count];
            double[] bids = new double[ticks.Count];

            double[] timestamps = new double[ticks.Count];

            double[] ssi_win = new double[ticks.Count];
            double[] ssi = new double[ticks.Count];
            double[] zero = new double[ticks.Count];


            int index = 0;
            foreach (Tickdata tick in ticks)
            {
                asks[index] = tick.ask;
                bids[index] = tick.bid;
                timestamps[index] = tick.timestamp;

                ssi_win[index] = database.getIndicator(tick.timestamp, "ssi-win-mt4", instrument).value;
                ssi[index] = database.getIndicator(tick.timestamp, "ssi-mt4", instrument).value;
                zero[index] = 0;

                index++;
            }

            // Add 3 data series to the line layer
            price_layer.addDataSet(asks, HexColorCodes.blue, "Ask");
            price_layer.addDataSet(bids, HexColorCodes.green, "Bid");

            oszilator_layer.addDataSet(ssi_win, HexColorCodes.red, "SSI-Win");
            oszilator_layer.addDataSet(ssi, HexColorCodes.pink, "SSI");
            oszilator_layer.addDataSet(zero, HexColorCodes.white, "Zero");

            // The x-coordinates for the line layer
            price_layer.setXData(timestamps);
            oszilator_layer.setXData(timestamps);

            // Output the chart
            viewer.Chart = c;
        }
    }
}
