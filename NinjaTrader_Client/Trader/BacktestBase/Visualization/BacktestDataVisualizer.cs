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

            foreach (KeyValuePair<long, BacktestVisualizationData> current in data.getVisualizationData())
            {
                long timestamp = current.Key;
                foreach(KeyValuePair<string, BacktestVisualizationDataComponent> component in current.Value.components)
                {
                    if (charts.ContainsKey(component.Value.chartId) == false)
                    {
                        int m = 1;
                        if (component.Value.chartId == 0)
                            m = 3;

                        CustomChart chart = null;

                        if(component.Value.type == BacktestVisualizationDataComponent.VisualizationType.OnChart)
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

                    charts[component.Value.chartId].addData(component.Value.getName(), timestamp, component.Value.value);
                }
            }

            Bitmap bmp = new Bitmap(width, realHeight);
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
                g.DrawString(chartNames[chartPair.Key], SystemFonts.DefaultFont, Brushes.White, 5, 5 + nextHeight);

                chartForTimeScale = chartPair.Value;

                nextHeight += chartPair.Value.getHeight();
            }

            foreach (TradePosition position in data.getPositions())
            {
                try {
                    Pen lineColor = (position.type == TradePosition.PositionType.longPosition ? Pens.Blue : Pens.DarkOliveGreen);

                    g.DrawLine(Pens.Gray, chartForTimeScale.getXTime(position.timestampClose), 0, chartForTimeScale.getXTime(position.timestampClose), bmp.Height);
                    g.DrawLine(lineColor, chartForTimeScale.getXTime(position.timestampOpen), 0, chartForTimeScale.getXTime(position.timestampOpen), bmp.Height);
                }
                catch (Exception) { }
            }

            return bmp;
        }
    }
}
