using NinjaTrader_Client.Trader.BacktestBase;
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
    public partial class TradeHistoryChartForm : Form
    {
        public TradeHistoryChartForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text != null && textBox1.Text != "")
            {
                List<TradePosition> positions = BacktestFormatter.getPositionHistoryFromCodedString(textBox1.Text);

                Bitmap bmp = new Bitmap(1000, 500);
                Graphics g = Graphics.FromImage(bmp);

                g.DrawLine(Pens.Black, 0, bmp.Height / 2, bmp.Width, bmp.Height / 2);

                double maxDif = double.MinValue;
                double minDif = double.MaxValue;

                long minTime = long.MaxValue;
                long maxTime = long.MinValue;

                foreach(TradePosition pos in positions)
                {
                    if(pos.timestampClose > maxTime)
                        maxTime = pos.timestampClose;

                    if(pos.timestampOpen < minTime)
                        minTime = pos.timestampOpen;

                    if(pos.getDifference() > maxDif)
                        maxDif = pos.getDifference();

                    if(pos.getDifference() < minDif)
                        minDif = pos.getDifference();
                }

                foreach(TradePosition pos in positions)
                {
                    long timeMiddle = pos.timestampOpen + ((pos.timestampClose - pos.timestampOpen) / 2);
                    int x = Convert.ToInt32(Convert.ToDouble(timeMiddle - minTime) / Convert.ToDouble(maxTime - minTime) * bmp.Width);
                    int y = Convert.ToInt32(Convert.ToDouble(Math.Abs(pos.getDifference()) - minDif) / Convert.ToDouble(maxDif - minDif) * bmp.Height / 2);

                    if (pos.getDifference() > 0)
                        y = y * -1;

                    y = y + bmp.Height / 2;

                    g.FillEllipse((pos.getDifference() > 0 ? Brushes.Green : Brushes.Red), x, y, 5, 5);
                }

                pictureBox1.Image = bmp;
            }
        }

        private void TradeHistoryChartForm_Load(object sender, EventArgs e)
        {

        }
    }
}
