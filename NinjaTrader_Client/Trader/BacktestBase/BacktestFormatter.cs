using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.BacktestBase
{
    class BacktestFormatter
    {
        public static string getCSVHeader(BacktestData data)
        {
            return getCSVHeader(data.getParameters(), data.getResult());
        }

        public static string getCSVHeader(Dictionary<string, string> parameterSet, Dictionary<string, string> resultSet)
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameterSet)
                output += pair.Key + ";";

            foreach (KeyValuePair<string, string> pair in resultSet)
                output += pair.Key + ";";

            return output;
        }

        public static string getCSVLine(BacktestData data)
        {
            return getCSVLine(data.getParameters(), data.getResult());
        }

        public static string getCSVLine(Dictionary<string, string> parameterSet, Dictionary<string, string> resultSet)
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameterSet)
                output += pair.Value + ";";

            foreach (KeyValuePair<string, string> pair in resultSet)
                output += pair.Value + ";";

            return output;
        }

        public static string getResultText(BacktestData data)
        {
            return getResultText(data.getResult());
        }

        public static string getResultText(Dictionary<string, string> resultSet)
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in resultSet)
                output += pair.Key + ": " + pair.Value + Environment.NewLine;
            return output;
        }

        public static string getPositionsText(BacktestData data)
        {
            return getPositionsText(data.getPositions());
        }

        public static string getPositionsText(List<TradePosition> trades)
        {
            string output = "";
            foreach (TradePosition position in trades)
                output += Math.Round(position.getDifference(), 4) + Environment.NewLine;

            return output;
        }

        public static string getParameterText(BacktestData data)
        {
            return getParameterText(data.getParameters());
        }

        public static string getParameterText(Dictionary<string, string> parameterSet)
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameterSet)
                output += pair.Key + ": " + pair.Value + Environment.NewLine;
            return output;
        }
    }
}
