using NinjaTrader_Client.Trader.TradingAPIs;
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
    public partial class LiveTradingForm : Form
    {
        private NTLiveTradingAPI api;
        private string pair = "USDJPY";
        public LiveTradingForm(NTLiveTradingAPI api)
        {
            InitializeComponent();
            this.api = api;
        }

        private void LiveTradingForm_Load(object sender, EventArgs e)
        {
            updateTimer.Start();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            labelInfo.Text = "Bid " + api.getBid(pair)
                + Environment.NewLine + "Ask " + api.getAsk(pair)
                + Environment.NewLine + "Long " + (api.getLongPosition(pair) != null ? (api.getBid(pair) - api.getLongPosition(pair).priceOpen) + "" : "NULL")
                + Environment.NewLine + "Short " + (api.getShortPosition(pair) != null ? (api.getShortPosition(pair).priceOpen) - api.getAsk(pair) + "" : "NULL")
                + Environment.NewLine + "Uptodate " + api.isUptodate(pair) + " (" + api.getNow() + ")";

                goLongBtn.Enabled = (api.getLongPosition(pair) == null);
                closeLongBtn.Enabled = (api.getLongPosition(pair) != null);

                goShortBtn.Enabled = (api.getShortPosition(pair) == null);
                closeShortBtn.Enabled = (api.getShortPosition(pair) != null);

        }

        private void goLongBtn_Click(object sender, EventArgs e)
        {
            api.openLong(pair);
        }

        private void closeLongBtn_Click(object sender, EventArgs e)
        {
            api.closeLong(pair);
        }

        private void closeAllBtn_Click(object sender, EventArgs e)
        {
            api.closePositions(pair);
        }

        private void goShortBtn_Click(object sender, EventArgs e)
        {
            api.openShort(pair);
        }

        private void closeShortBtn_Click(object sender, EventArgs e)
        {
            api.closeShort(pair);
        }
    }
}
