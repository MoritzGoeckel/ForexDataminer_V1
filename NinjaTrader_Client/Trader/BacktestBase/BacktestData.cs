using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Backtest
{
    public class BacktestData
    {
        //Export these! ???
        private Dictionary<string, string> parameterSet = new Dictionary<string, string>();
        private Dictionary<string, string> resultSet = new Dictionary<string, string>();
        private List<TradePosition> trades = new List<TradePosition>(); //Generate grahpic

        private long hours;
        private string pair, strategy;

        //Generate Chart? Optional?

        public BacktestData(long timeframe, string pair, string strategy)
        {
            this.pair = pair;
            this.hours = timeframe / 1000 / 60 / 60;
            this.strategy = strategy;

            setParameter("pair", pair);
            setParameter("timeframe", hours.ToString());
            setParameter("strategy", strategy);
        }

        public void setPositions(List<TradePosition> positions)
        {
            trades.AddRange(positions);

            double profit = 0;
            double drawdown = 999999999;
            int longPositions = 0, winPositions = 0;

            long holdTime = 0;

            double lastDayProfit = 0;
            long lastDayStart = 0;

            int positiveDays = 0;
            int days = 0;

            if (positions.Count != 0)
            {
                lastDayStart = positions[0].timestampOpen;

                foreach (TradePosition position in trades)
                {
                    profit += position.getDifference();
                    holdTime += position.timestampClose - position.timestampOpen;

                    if (profit < drawdown)
                        drawdown = profit;

                    if (position.type == TradePosition.PositionType.longPosition)
                        longPositions++;

                    if (position.getDifference() > 0)
                        winPositions++;

                    if (position.timestampClose > lastDayStart + 1000 * 60 * 60 * 24)
                    {
                        if (profit >= lastDayProfit)
                            positiveDays++;

                        days++;

                        lastDayProfit = profit;
                        while (lastDayStart < position.timestampClose)
                            lastDayStart += 1000 * 60 * 60 * 24;
                    }
                }
            }

            if (drawdown > 0)
                drawdown = 0;

            setResult("positions", trades.Count.ToString());
            setResult("profit", profit.ToString());
            setResult("drawdown", drawdown.ToString());
            setResult("long", ((double)longPositions / (double)trades.Count).ToString());
            setResult("winRatio", ((double)winPositions / (double)trades.Count).ToString());
            setResult("positiveDaysRatio", ((double)positiveDays / (double)days).ToString());

            //Standart Deviation
            double sumNegative = 0, sumPositive = 0, sum = 0;
            double countNegative = 0, countPositive = 0, count = 0;
            double varianceSumNegative = 0, varianceSumPositive = 0, varianceSum = 0;

            foreach (TradePosition trade in trades)
            {
                if(trade.getDifference() > 0)
                {
                    sumPositive += trade.getDifference();
                    countPositive++;
                }

                if(trade.getDifference() < 0)
                {
                    sumNegative += trade.getDifference();
                    countNegative++;
                }

                if (trade.getDifference() != 0)
                {
                    sum += trade.getDifference();
                    count++;
                }
            }

            double meanNegative = sumNegative / countNegative;
            double meanPositive = sumPositive / countPositive;
            double mean = sum / count;

            setResult("meanLoss", meanNegative.ToString());
            setResult("meanWin", meanPositive.ToString());
            setResult("meanTrade", mean.ToString());

            foreach (TradePosition trade in trades)
            {
                if (trade.getDifference() > 0)
                    varianceSumPositive += Math.Pow(trade.getDifference() - meanPositive, 2);

                if (trade.getDifference() < 0)
                    varianceSumNegative += Math.Pow(trade.getDifference() - meanNegative, 2);

                if (trade.getDifference() != 0)
                    varianceSum += Math.Pow(trade.getDifference() - mean, 2);
            }

            double stdDeviationNegative = Math.Sqrt(varianceSumNegative / countNegative);
            double stdDeviationPositive = Math.Sqrt(varianceSumPositive / countPositive);
            double stdDeviation = Math.Sqrt(varianceSum / count);

            setResult("stdDeviationWin", stdDeviationPositive.ToString());
            setResult("stdDeviationLoss", stdDeviationNegative.ToString());
            setResult("stdDeviation", stdDeviation.ToString());

            //Ende Standart Deviation

            if (positions.Count != 0)
                setResult("meanHoldtime", (holdTime / positions.Count / 1000 / 60).ToString());
        }

        public string getResult(string key)
        {
            return resultSet[key.ToLower()];
        }

        public List<TradePosition> getPositions()
        {
            return trades;
        }

        public void setParameter(Dictionary<string, string> parameter)
        {
            foreach (KeyValuePair<string, string> pair in parameter)
                if (this.parameterSet.ContainsKey(pair.Key) == false)
                    this.parameterSet.Add(pair.Key, pair.Value);
                else
                    this.parameterSet[pair.Key] = pair.Value;
        }

        public void setParameter(string key, string value)
        {
            if (parameterSet.ContainsKey(key))
                parameterSet[key] = value;
            else
                parameterSet.Add(key, value);
        }

        public Dictionary<string, string> getParameters()
        {
            return parameterSet;
        }

        public void setResult(string key, string value)
        {
            key = key.ToLower();
            if (resultSet.ContainsKey(key))
                resultSet[key] = value;
            else
                resultSet.Add(key, value);
        }

        public void setResult(Dictionary<string, string> results)
        {
            foreach (KeyValuePair<string, string> pair in results)
                if (this.resultSet.ContainsKey(pair.Key) == false)
                    this.resultSet.Add(pair.Key, pair.Value);
                else
                    this.resultSet[pair.Key] = pair.Value;
        }

        public Dictionary<string, string> getResult()
        {
            return resultSet;
        }
    }
}
