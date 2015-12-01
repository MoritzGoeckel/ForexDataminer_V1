using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader.Analysis
{
    public partial class CorrelationAnalysisForm : Form
    {
        private Database database;

        private long start, end;
        private int resolution;
        private List<string> pairs;

        private int runningThreads = 0;
        private int threadsCount = 3;

        public CorrelationAnalysisForm(Database database, int resolution, long days, List<string> pairs)
        {
            this.database = database;
            this.end = database.getLastTimestamp();
            this.start = end - (days * 24L * 60L * 60L * 1000L);
            this.resolution = resolution;
            this.pairs = pairs;

            InitializeComponent();
        }

        private void CorrelationAnalysisForm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < pairs.Count(); i++)
            {
                for (int j = i + 1; j < pairs.Count(); j++)
                {
                    Thread thread = new Thread(() => doPair(pairs[i], pairs[j])); ;
                    thread.Start();

                    do
                        Thread.Sleep(100);
                    while (runningThreads >= threadsCount);
                }
            }
        }

        private Bitmap doPair(string thisPair, string otherPair)
        {
            runningThreads++;

            string path = Application.StartupPath + "/backtestes/" + thisPair + "-" + otherPair + ".png";
            if (File.Exists(path))
            {
                runningThreads--;
                return null;
            }

            long currentTime = start;
            long step = (end - start) / (resolution * resolution);

            //This MINMax
            double thisPairMax = 0, thisPairMin = double.MaxValue;
            foreach (Tickdata data in database.getPrices(start, end, thisPair))
            {
                double price = data.getAvg();
                if (price > thisPairMax)
                    thisPairMax = price;
                if (price < thisPairMin)
                    thisPairMin = price;
            }

            //Other minmax
            double otherPairMax = 0, otherPairMin = double.MaxValue;
            foreach (Tickdata data in database.getPrices(start, end, otherPair))
            {
                double price = data.getAvg();
                if (price > otherPairMax)
                    otherPairMax = price;
                if (price < otherPairMin)
                    otherPairMin = price;
            }

            Bitmap bmp = new Bitmap(Convert.ToInt32(resolution), Convert.ToInt32(resolution));
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

            g.DrawString("0", DefaultFont, Brushes.Black, new PointF(0, 0));
            g.DrawString("1", DefaultFont, Brushes.Black, new PointF(resolution - 20, resolution - 20));

            g.DrawString("Min: "
                + thisPair + "=" + Math.Round(thisPairMin, 4)
                + " "
                + otherPair + "=" + Math.Round(otherPairMin, 4)
                + Environment.NewLine
                + "Max: "
                + thisPair + "=" + Math.Round(thisPairMax, 4)
                + " "
                + otherPair + "=" + Math.Round(otherPairMax, 4),
                DefaultFont, Brushes.Black, new PointF(30, 5));

            g.DrawString(thisPair, DefaultFont, Brushes.Black, new PointF(resolution - 70, 5));
            g.DrawString(otherPair, DefaultFont, Brushes.Black, new PointF(5, resolution - 20));

            while (currentTime < end)
            {
                double thePrice = database.getPrice(currentTime, thisPair, false).getAvg() - thisPairMin;
                double theRatio = thePrice / (thisPairMax - thisPairMin);

                double otherPrice = database.getPrice(currentTime, otherPair, false).getAvg() - otherPairMin;
                double otherRatio = otherPrice / (otherPairMax - otherPairMin);

                int x = Convert.ToInt32(theRatio * resolution);
                int y = Convert.ToInt32(otherRatio * resolution);

                if (x >= resolution)
                    x = resolution - 1;
                if (x <= 0)
                    x = 1;
                if (y >= resolution)
                    y = resolution - 1;
                if (y <= 0)
                    y = 1;

                //g.DrawEllipse(Pens.Black, x - (size / 2), y - (size / 2), size, size);
                Color c = bmp.GetPixel(x, y);

                Color nColor = Color.FromArgb(
                    (c.R > 50 ? c.R - 10 : 40),
                    (c.G > 50 ? c.G - 10 : 40),
                    (c.B > 50 ? c.B - 10 : 40));

                bmp.SetPixel(x, y, nColor);

                currentTime += step;

                if(currentTime >= end)
                {
                    //leztes
                    bmp.SetPixel(x, y, Color.Red);
                    g.DrawEllipse(Pens.Red, x - 3, y - 3, 6, 6);
                }

                double percent = Convert.ToDouble(currentTime - start) / Convert.ToDouble(end - start) * 100d;
                string t = percent.ToString();
            }

            bmp.Save(path);

            runningThreads--;

            return bmp;
        }
    }
}
