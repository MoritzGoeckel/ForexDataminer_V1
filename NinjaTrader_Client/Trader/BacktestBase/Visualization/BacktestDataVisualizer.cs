using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Charting;
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

            foreach (KeyValuePair<long, BacktestVisualizationData> current in data.getVisualizationData())
            {
                long timestamp = current.Key;
                foreach(KeyValuePair<string, BacktestVisualizationDataComponent> component in current.Value.components)
                {
                    if (charts.ContainsKey(component.Value.chartId) == false)
                    {
                        int m = 1;
                        if (component.Value.chartId == 0)
                            m = 2;

                        if(component.Value.type == BacktestVisualizationDataComponent.VisualizationType.OnChart)
                            charts.Add(component.Value.chartId, new CustomChart(width, oneChartHeight * m));

                        if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.TrafficLight)
                            charts.Add(component.Value.chartId, new CustomChart(width, oneChartHeight / 4 * m, 0, 1, true));

                        if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.OneToMinusOne)
                            charts.Add(component.Value.chartId, new CustomChart(width, oneChartHeight * m, -1, 1));

                        if (component.Value.type == BacktestVisualizationDataComponent.VisualizationType.ZeroToOne)
                            charts.Add(component.Value.chartId, new CustomChart(width, oneChartHeight * m, 0, 1));

                        chartNames.Add(component.Value.chartId, component.Value.getName());
                    }

                    charts[component.Value.chartId].addData(component.Value.getName(), timestamp, component.Value.value);
                }
            }

            Bitmap bmp = new Bitmap(width, oneChartHeight * charts.Count);
            Graphics g = Graphics.FromImage(bmp);

            int nextHeight = 0;
            foreach(KeyValuePair<int, CustomChart> chartPair in charts)
            {
                if(chartPair.Key == 0)
                    g.DrawImage(chartPair.Value.drawChart(data.getPositions()), 0, nextHeight);
                else
                    g.DrawImage(chartPair.Value.drawChart(), 0, nextHeight);

                g.DrawLine(Pens.LightGray, 0, nextHeight, bmp.Width, nextHeight);
                g.DrawString(chartNames[chartPair.Key], SystemFonts.DefaultFont, Brushes.White, 5, 5 + nextHeight);

                nextHeight += chartPair.Value.getHeight();
            }

            return bmp;
        }
    }
}
