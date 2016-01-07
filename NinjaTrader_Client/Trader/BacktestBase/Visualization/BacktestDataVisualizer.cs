using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Charting;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.BacktestBase.Visualization
{
    class BacktestDataVisualizer
    {
        public static Image getImageFromBacktestData(BacktestData data, int width, int oneChartHeight)
        {
            Dictionary<int, CustomChart> charts = new Dictionary<int, CustomChart>();
            Dictionary<int, string> chartNames = new Dictionary<int, string>();

            int realHeight = 0;

            foreach (KeyValuePair<string, BacktestVisualizationDataComponent> component in data.getVisualizationData()[0].Value.components)
            {
                if (charts.ContainsKey(component.Value.chartId) == false)
                {
                    int m = 1;
                    if (component.Value.chartId == 0)
                        m = 3;

                    CustomChart chart = null;

                    if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.OnChart)
                        chart = new CustomChart(width, oneChartHeight * m);

                    if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.TrafficLight)
                        chart = new CustomChart(width, oneChartHeight / 6 * m, 0, 1, true);

                    if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne)
                        chart = new CustomChart(width, oneChartHeight * m, -1, 1);

                    if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.ZeroToOne)
                        chart = new CustomChart(width, oneChartHeight * m, 0, 1);

                    charts.Add(component.Value.chartId, chart);
                    chartNames.Add(component.Value.chartId, component.Value.getName());

                    realHeight += chart.getHeight();
                }
                else
                {
                    chartNames[component.Value.chartId] += " " + component.Value.getName();
                }
            }

            foreach (KeyValuePair<long, BacktestVisualizationData> current in data.getVisualizationData())
            {
                long timestamp = current.Key;
                foreach(KeyValuePair<string, BacktestVisualizationDataComponent> component in current.Value.components)
                {
                    charts[component.Value.chartId].addData(component.Value.getName(), timestamp, component.Value.value);
                }
            }

            Bitmap bmp = new Bitmap(width, realHeight + 100);
            Graphics g = Graphics.FromImage(bmp);

            CustomChart chartForTimeScale = null;

            int nextHeight = 0;
            foreach(KeyValuePair<int, CustomChart> chartPair in charts)
            {
                if(chartPair.Key == 0)
                    g.DrawImage(chartPair.Value.drawChart(data.getPositions()), 0, nextHeight);
                else
                    g.DrawImage(chartPair.Value.drawChart(), 0, nextHeight);

                g.DrawLine(Pens.DarkGray, 0, nextHeight, bmp.Width, nextHeight);
                g.DrawString(chartNames[chartPair.Key], SystemFonts.DialogFont, Brushes.Black, 5, 5 + nextHeight);

                chartForTimeScale = chartPair.Value;

                nextHeight += chartPair.Value.getHeight();
            }

            double minProfit = double.MaxValue, maxProfit = double.MinValue;
            foreach (TradePosition position in data.getPositions())
            {
                try {
                    Pen lineColor = (position.type == TradePosition.PositionType.longPosition ? Pens.Blue : Pens.DarkOliveGreen);

                    g.DrawLine(Pens.Gray, chartForTimeScale.getXTime(position.timestampClose), 0, chartForTimeScale.getXTime(position.timestampClose), bmp.Height);
                    g.DrawLine(lineColor, chartForTimeScale.getXTime(position.timestampOpen), 0, chartForTimeScale.getXTime(position.timestampOpen), bmp.Height);
                }
                catch (Exception) { }

                if (Math.Abs(position.getDifference()) > maxProfit)
                    maxProfit = Math.Abs(position.getDifference());

                if (position.getDifference() < minProfit)
                    minProfit = Math.Abs(position.getDifference());
            }

            int y = bmp.Height - 50;
            g.DrawLine(Pens.Gray, 0, y, bmp.Width, y);
            g.DrawString("Positions", SystemFonts.DialogFont, Brushes.Black, 5, bmp.Height - 100 + 5);

            foreach (TradePosition position in data.getPositions())
            {
                Brush blockColor = (position.getDifference() > 0 ? Brushes.Green : Brushes.Red);

                double ratio = (Math.Abs(position.getDifference()) - minProfit) / (maxProfit - minProfit);

                int height = Convert.ToInt32(ratio * 50 - 5);

                int yOffset = 0;
                if (position.getDifference() > 0)
                    yOffset = height;

                g.FillRectangle(blockColor, chartForTimeScale.getXTime(position.timestampOpen), y - yOffset, chartForTimeScale.getXTime(position.timestampClose) - chartForTimeScale.getXTime(position.timestampOpen), height);
            }

            return bmp;
        }
    }
}
