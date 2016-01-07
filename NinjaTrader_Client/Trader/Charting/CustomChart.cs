using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Charting
{
    class CustomChart
    {
        Dictionary<string, List<KeyValuePair<long, double>>> data = new Dictionary<string, List<KeyValuePair<long, double>>>();

        double min_value = double.MaxValue, max_value = double.MinValue;
        long min_time = long.MaxValue, max_time = long.MinValue;

        double fixMin_value, fixMax_value;

        int width, height;

        bool trafficLightMode = false;

        public CustomChart(int width, int height, double fixMin_value = double.NaN, double fixMax_value = double.NaN, bool trafficLightMode = false)
        {
            this.height = height;
            this.width = width;

            this.trafficLightMode = trafficLightMode;

            this.fixMax_value = fixMax_value;
            this.fixMin_value = fixMin_value;

            if (double.IsNaN(fixMax_value) == false && double.IsNaN(fixMin_value) == false)
            {
                min_value = fixMin_value;
                max_value = fixMax_value;
            }
        }

        public void addData(string name, long time, double value)
        {
            if(data.ContainsKey(name) == false)
            {
                data.Add(name, new List<KeyValuePair<long, double>>());
            }

            data[name].Add(new KeyValuePair<long, double>(time, value));

            if (time > max_time)
                max_time = time;

            if (time < min_time)
                min_time = time;

            if (double.IsNaN(fixMax_value) || double.IsNaN(fixMin_value))
            {
                if (value > max_value)
                    max_value = value;

                if (value < min_value)
                    min_value = value;
            }
        }

        double valueOffset = 0;
        public Image drawChart(List<TradePosition> history = null)
        {
            if (min_value < 0)
            {
                max_value = max_value + Math.Abs(min_value);
                valueOffset = Math.Abs(min_value);
                min_value = 0;
            }

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            //g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

            foreach (KeyValuePair<string, List<KeyValuePair<long, double>>> dataComponent in data)
            {
                foreach (KeyValuePair<long, double> timeValuePair in dataComponent.Value)
                {
                    string name = dataComponent.Key;
                    long time = timeValuePair.Key;
                    double value = timeValuePair.Value;

                    try {
                        if (double.IsNaN(value) == false)
                        {
                            int x = getXTime(time);

                            if (x == bmp.Width)
                                x = bmp.Width - 1;

                            if (x == 0)
                                x = 1;

                            double y = value;

                            if (trafficLightMode == false)
                            {
                                value += valueOffset;

                                y = getYValue(value);

                                if (y == 0)
                                    y = 1;

                                if (y == bmp.Height)
                                    y = bmp.Height - 1;
                            }

                            if (x < 0 || y < 0 || y > bmp.Height || x > bmp.Width)
                                throw new Exception("Out of bounds! X: " + x + " Y: " + y + " min: " + min_value);

                            if (trafficLightMode)
                            {
                                if (y == 0)
                                    g.FillRectangle(Brushes.LightPink, x, 0, 1, bmp.Height);
                                else if (y == 1)
                                    g.FillRectangle(Brushes.LightGreen, x, 0, 1, bmp.Height);
                                else if (y == 0.5)
                                    g.FillRectangle(Brushes.LightYellow, x, 0, 1, bmp.Height);
                            }
                            else
                                bmp.SetPixel(x, Convert.ToInt32(y), Color.Black); //??? Color?
                        }
                    }
                    catch (Exception) { }
                }
            }

            //Draw history if possible
            if (history != null)
            {
                foreach (TradePosition position in history)
                {
                    int fourthOfHeight = bmp.Height / 4;

                    Pen lineColor = (position.getDifference() > 0 ? Pens.Green : Pens.Red);
                    g.DrawLine(lineColor, getXTime(position.timestampOpen), getYValue(position.priceOpen), getXTime(position.timestampClose), getYValue(position.priceClose));

                    //g.DrawLine(Pens.Gray, getXTime(position.timestampOpen), fourthOfHeight, getXTime(position.timestampOpen), bmp.Height - fourthOfHeight);
                    //g.DrawLine(lineColor, getXTime(position.timestampClose), fourthOfHeight, getXTime(position.timestampClose), bmp.Height - fourthOfHeight);
                }
            }

            return bmp;
        }

        internal int getHeight()
        {
            return height;
        }

        public int getYValue(double value)
        {
            double devisor = max_value - min_value;

            if (devisor == 0)
                throw new Exception("Devide by 0");

            return Convert.ToInt32((1 - ((value - min_value) / devisor)) * height);
        }

        public int getXTime(long time)
        {
            return Convert.ToInt32((Convert.ToDouble(time - min_time) / Convert.ToDouble(max_time - min_time)) * Convert.ToDouble(width));
        }
    }
}
