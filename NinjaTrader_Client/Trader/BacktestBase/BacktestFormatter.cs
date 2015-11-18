using NinjaTrader_Client.Trader.Backtest;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Text;

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

            output += "stringCodedParams;stringCodedPositions;stringCodedResult;";

            return output;
        }

        public static string getCSVLine(BacktestData data)
        {
            StringBuilder output = new StringBuilder("");

            foreach (KeyValuePair<string, string> pair in data.getParameters())
                output.Append(pair.Value + ";");

            foreach (KeyValuePair<string, string> pair in data.getResult())
                output.Append(pair.Value + ";");

            output.Append(BacktestFormatter.getDictStringCoded(data.getParameters()) + ";");
            output.Append(BacktestFormatter.getStringCodedPositionHistory(data.getPositions()) + ";");
            output.Append(BacktestFormatter.getDictStringCoded(data.getResult()) + ";");

            return output.ToString();
        }

        public static string getStringCodedPositionHistory(List<TradePosition> positions)
        {
            StringBuilder output = new StringBuilder("");

            if (positions.Count < 1000)
            {
                foreach (TradePosition pos in positions)
                    output.Append((pos.type == TradePosition.PositionType.longPosition ? "L" : "S") + ":" + pos.timestampOpen + ":" + pos.priceOpen + ":" + pos.timestampClose + ":" + pos.priceClose + "|");
            }
            else
                output.Append("More than 1000");

            return output.ToString();
        }

        public static List<TradePosition> getPositionHistoryFromCodedString(string str)
        {
            List<string> positions = new List<string>();
            positions.AddRange(str.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            List<TradePosition> positionList = new List<TradePosition>();

            foreach (string posString in positions)
            {
                try
                {
                    string[] posArray = posString.Split(':');
                    positionList.Add(new TradePosition((posArray[0] == "L" ? TradePosition.PositionType.longPosition : TradePosition.PositionType.shortPosition), long.Parse(posArray[1]), double.Parse(posArray[2]), long.Parse(posArray[3]), double.Parse(posArray[4])));
                }
                catch (Exception e){ }
            }

            return positionList;
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

        public static string getParameterStringCoded(BacktestData data)
        {
            return getDictStringCoded(data.getParameters());
        }

        public static string getDictStringCoded(Dictionary<string, string> parameterSet)
        {
            string output = "";
            foreach (KeyValuePair<string, string> pair in parameterSet)
                output += pair.Key + ":" + pair.Value + "|";
            return output.Substring(0, output.Length - 1);
        }

        public static Dictionary<string, string> convertStringCodedToParameters(string str)
        {
            List<string> parameters = new List<string>();
            parameters.AddRange(str.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            Dictionary<string, string> paramDict = new Dictionary<string,string>();

            foreach(string parameter in parameters)
            {
                string[] pair = parameter.Split(':');
                paramDict.Add(pair[0], pair[1]);
            }

            return paramDict;
        }
    }
}
