using ChartDirector;
using NinjaTrader_Client.Trader;
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

        private void ChartingForm_Load(object sender, EventArgs e)
        {
            WinChartViewer viewer = new WinChartViewer();
            viewer.BackColor = Color.Black;
            viewer.Dock = DockStyle.Fill;

            this.Controls.Add(viewer);

            XYChart c = new XYChart(600, 400);
            c.setColor(Chart.TextColor, Color.White.GetHashCode());

            // Add a title box using grey (0x555555) 20pt Arial font
            c.addTitle("Testchart", "Arial", 20, Color.White.GetHashCode());

            // Set the plotarea at (70, 70) and of size 500 x 300 pixels, with transparent
            // background and border and light grey (0xcccccc) horizontal grid lines
            c.setPlotArea(70, 70, 500, 300, Chart.Transparent, -1, Chart.Transparent, Color.Black.GetHashCode());

            // Add a legend box with horizontal layout above the plot area at (70, 35). Use 12pt
            // Arial font, transparent background and border, and line style legend icon.
            LegendBox b = c.addLegend(70, 35, false, "Arial", 12);
            b.setBackground(Chart.Transparent, Chart.Transparent);
            b.setLineStyleKey();

            // Set axis label font to 12pt Arial
            c.xAxis().setLabelStyle("Arial", 12);
            c.yAxis().setLabelStyle("Arial", 12);

            // Set the x and y axis stems to transparent, and the x-axis tick color to grey
            // (0xaaaaaa)
            c.xAxis().setColors(Chart.Transparent, Chart.TextColor, Chart.TextColor, 0xaaaaaa);
            c.yAxis().setColors(Chart.Transparent);

            // Set the major/minor tick lengths for the x-axis to 10 and 0.
            c.xAxis().setTickLength(10, 0);

            // For the automatic axis labels, set the minimum spacing to 80/40 pixels for the x/y
            // axis.
            c.xAxis().setTickDensity(80);
            c.yAxis().setTickDensity(40);

            // Add a title to the y axis using dark grey (0x555555) 14pt Arial font
            c.yAxis().setTitle("Y-Axis Title Placeholder", "Arial", 14, Color.White.GetHashCode());

            // Add a line layer to the chart with 3-pixel line width
            LineLayer layer = c.addLineLayer2();
            layer.setLineWidth(2);

            //Get the data database ???

            // Add 3 data series to the line layer
            //layer.addDataSet(data0, 0x5588cc, "Alpha");
            //layer.addDataSet(data1, 0xee9944, "Beta");
            //layer.addDataSet(data2, 0x99bb55, "Gamma");

            // The x-coordinates for the line layer
            //layer.setXData(timeStamps);

            // Output the chart
            viewer.Chart = c;
        }
    }
}
