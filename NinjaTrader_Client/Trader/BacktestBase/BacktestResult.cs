using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtest
{
    public class BacktestResult
    {
        public Dictionary<string, string> parameter = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, BacktestResult>> pairResults = new Dictionary<string, Dictionary<string, BacktestResult>>();

        public Dictionary<string, string> results = new Dictionary<string, string>();

        private List<TradePosition> trades = new List<TradePosition>();

        private long hours;
        private string pair, strategy;

        //Generate Chart? Optional?

        public BacktestResult(long timeframe, string pair, string strategy)
        {
            this.pair = pair;
            this.hours = timeframe / 1000 / 60 / 60;
            this.strategy = strategy;

            setParameter("Pair", pair);
            setParameter("Time", hours.ToString());
            setParameter("Strategy", strategy);
        }

        public void setResult(string key, string value)
        {
            key = key.ToLower();
            if (results.ContainsKey(key))
                results[key] = value;
            else
                results.Add(key, value);
        }

        public void addPositions(List<TradePosition> positions)
        {
            trades.AddRange(positions);

            double profit = 0;
            double drawdown = 999999999;
            int longPositions = 0, winPositions = 0;

            long holdTime = 0;

            foreach(TradePosition position in trades)
            {
                profit += position.getDifference();
                holdTime += position.timestampClose - position.timestampOpen;

                if (profit < drawdown)
                    drawdown = profit;

                if (position.type == TradePosition.PositionType.longPosition)
                    longPositions++;

                if (position.getDifference() > 0)
                    winPositions++;
            }

            setResult("Positions", trades.Count.ToString());
            setResult("Profit", profit.ToString());
            setResult("Drawdown", drawdown.ToString());
            setResult("Long", ((double)longPositions / (double)trades.Count).ToString());
            setResult("Winning", ((double)winPositions / (double)trades.Count).ToString());
            setResult("Pips/Position", (profit / (double)trades.Count).ToString());
            setResult("Pips/Day", (profit / (double)hours * 24d).ToString());
            setResult("Positions/Day", ((double)positions.Count / (double)hours * 24d).ToString());

            if (positions.Count != 0)
                setResult("Holdtime/Positions", (holdTime / positions.Count / 1000 / 60).ToString());
        }

        public string getResult(string key)
        {
            return results[key.ToLower()];
        }

        public string getCSVHeader()
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameter)
                output += pair.Key + ";";

            foreach (KeyValuePair<string, string> pair in results)
                output += pair.Key + ";";

            return output;
        }

        public string getCSV()
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameter)
                output += pair.Value + ";";

            foreach (KeyValuePair<string, string> pair in results)
                output += pair.Value + ";";

            return output;
        }

        public string getResultText()
        {
            string output = "";
            foreach(KeyValuePair<string, string> pair in results)
                output += pair.Key + ": " + pair.Value + Environment.NewLine;
            return output;
        }

        public string getTradesText()
        {
            string output = "";
            foreach(TradePosition position in trades)
                output += Math.Round(position.getDifference(), 4) + Environment.NewLine;

            return output;
        }

        public List<TradePosition> getPositions()
        {
            return trades;
        }

        public void setParameter(string key, string value)
        {
            if (parameter.ContainsKey(key))
                parameter[key] = value;
            else
                parameter.Add(key, value);
        }

        public string getParameterText()
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameter)
                output += pair.Key + ": " + pair.Value + Environment.NewLine;
            return output;
        }
    }
}
