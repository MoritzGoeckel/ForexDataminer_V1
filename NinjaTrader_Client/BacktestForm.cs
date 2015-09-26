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

        int backtestHours = 48;
        string pair = "EURUSD";

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            long endTimestamp = database.getLastTimestamp();
            long startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);

            BacktestTradingAPI api = new BacktestTradingAPI(startTimestamp, database, pair);
            Strategy strat = new SSI_Strategy(database, api, pair);

            long currentTimestamp = startTimestamp;
            while(currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);
                strat.doTick();

                currentTimestamp += 1000 * 20;
            }

            var list = api.getHistory(); 
            double result = 0, drawdown = 99999999d;
            foreach(TradePosition p in list)
            {
                result += p.getDifference();

                if(result < drawdown)
                    drawdown = result;

                listBox1.Items.Add((p.type == TradePosition.PositionType.longPosition ? "L" : "S") + "   Change: " + Math.Round(p.getDifference(), 7) + "\t Total: " + Math.Round(result, 7));
            }

            label_info.Text = "Gained Pips: \t" + result + Environment.NewLine +
                "Positions: \t" + list.Count + Environment.NewLine +
                "Positions / day: \t" + (double)list.Count / (double)backtestHours * 24 + Environment.NewLine +
                "Pips / day: \t" + result / (double)backtestHours * 24d + Environment.NewLine +
                "Drawdown: \t" + drawdown + Environment.NewLine
                + Environment.NewLine +
                "Pair: \t" + pair + Environment.NewLine +
                "Period in h: \t" + backtestHours + Environment.NewLine + 
                "Strategy: " + strat.getName();

            //Output
            //Grafisch aufbereiten
            //(Pricechart, in/out)
            ChartingForm chartingForm = new ChartingForm(database, api.getHistory());
            chartingForm.Show();
        }
    }
}
