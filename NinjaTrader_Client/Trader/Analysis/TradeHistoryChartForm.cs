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
                //Draw em!
                MessageBox.Show("yup");
            }
        }
    }
}
