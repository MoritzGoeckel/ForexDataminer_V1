using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader
{
    public partial class StrategyStringGeneratorForm : Form
    {
        public StrategyStringGeneratorForm()
        {
            InitializeComponent();
        }

        private void StrategyStringGenerator_Load(object sender, EventArgs e)
        {
            Strategy strat = new SSIStrategy(null, 0.3, 0.2, false);
            textBox1.Text = BacktestFormatter.getDictStringCoded(strat.getParameters());
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Config.startupPath;

            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, textBox1.Text);
        }
    }
}
