using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Model;
using NinjaTrader_Client.Trader.Strategies;
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
    public partial class BacktestForm : Form
    {
        Database database;
        public BacktestForm(Database database)
        {
            InitializeComponent();
            this.database = database;
        }

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            long endTimestamp = database.getLastTimestamp();
            long startTimestamp = endTimestamp - (48 * 60 * 60 * 1000);

            BacktestTradingAPI api = new BacktestTradingAPI(startTimestamp, database, "EURUSD");
            Strategy strat = new SSI_Strategy(database, api, "EURUSD");

            long currentTimestamp = startTimestamp;
            while(currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);
                strat.doTick();

                currentTimestamp += 1000 * 20;
            }

            var list = api.getHistory(); 
            double result = 0;
            foreach(TradePosition p in list)
            {
                result += p.getDifference();
                listBox1.Items.Add("Change: " + Math.Round(p.getDifference(), 7) + "\t Total: " + Math.Round(result, 7));
            }

            //Output
            //Grafisch aufbereiten
            //(Pricechart, in/out)

            int i = 0;
        }
    }
}
