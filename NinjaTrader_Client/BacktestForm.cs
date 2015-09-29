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

        int backtestHours = 48 * 3;
        string pair = "EURUSD";

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            long endTimestamp = database.getLastTimestamp();
            long startTimestamp = endTimestamp - (backtestHours * 60 * 60 * 1000);

            BacktestTradingAPI api = new BacktestTradingAPI(startTimestamp, database, pair);
            //Strategy strat = new SSI_Strategy(database, api, pair);
            Strategy strat = new FastMovement_Strategy(database, api, pair);

            long currentTimestamp = startTimestamp;
            while(currentTimestamp < endTimestamp)
            {
                api.setNow(currentTimestamp);

                if (api.isUptodate()) //Dataset not older than 3 minutes
                    strat.doTick();

                currentTimestamp += 1000 * 20;
            }
            api.closePositions();

            string tradesStr = "";
            var positions = api.getHistory(); 
            double profit = 0, drawdown = 99999999d;

            int winPositions = 0;
            int longPositions = 0;

            foreach(TradePosition p in positions)
            {
                profit += p.getDifference();

                if(profit < drawdown)
                    drawdown = profit;

                if (p.type == TradePosition.PositionType.longPosition)
                    longPositions++;

                if (p.getDifference() > 0)
                    winPositions++;

                tradesStr += (p.type == TradePosition.PositionType.longPosition ? "L" : "S") + " d: " + Math.Round(p.getDifference(), 7) + "\t Total: " + Math.Round(profit, 7) + Environment.NewLine;
            }

            label_trades.Text = tradesStr;

            label_info.Text = "Gained Pips: \t" + Math.Round(profit * 10000, 2) + Environment.NewLine +
                "Positions:\t" + positions.Count + Environment.NewLine +
                "Positions / day:\t" + Math.Round((double)positions.Count / (double)backtestHours * 24d, 2) + Environment.NewLine +
                "Pips / day:\t" + Math.Round(profit * 10000d / (double)backtestHours * 24d, 2) + Environment.NewLine +
                "Pips / Position:\t" + Math.Round(profit * 10000d / (double)positions.Count, 2) + Environment.NewLine +
                "Win Positions:\t" + Math.Round((double)winPositions / (double)positions.Count, 2) + Environment.NewLine +
                "Long Positions:\t" + Math.Round((double)longPositions / (double)positions.Count, 2) + Environment.NewLine +
                "Drawdown:\t" + Math.Round(drawdown * 10000, 2) + Environment.NewLine +
                Environment.NewLine +
                "Pair:\t" + pair + Environment.NewLine +
                "Time (h):\t" + backtestHours + Environment.NewLine + 
                "Strategy:\t" + strat.getName() + Environment.NewLine + 
                Environment.NewLine +
                strat.getCustomResults();

            ChartingForm chartingForm = new ChartingForm(database, api.getHistory(), database.getLastTimestamp() - 1000 * 60 * 60 * backtestHours, database.getLastTimestamp());
            chartingForm.Show();
        }
    }
}
