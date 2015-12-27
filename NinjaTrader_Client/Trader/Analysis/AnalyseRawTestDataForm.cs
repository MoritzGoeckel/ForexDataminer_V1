using NinjaTrader_Client.Trader.BacktestBase;
using NinjaTrader_Client.Trader.Model;
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

namespace NinjaTrader_Client.Trader.Analysis
{
    public partial class AnalyseRawTestDataForm : Form
    {
        public AnalyseRawTestDataForm(bool onlyParameters)
        {
            this.onlyParameters = onlyParameters;
            InitializeComponent();
        }

        List<SimpleStrategyResult> data = new List<SimpleStrategyResult>();

        bool onlyParameters;

        private class SimpleStrategyResult
        {
            public Dictionary<string, string> result = new Dictionary<string, string>();
            public Dictionary<string, string> parameter = new Dictionary<string, string>();
            public double score;

            public SimpleStrategyResult(Dictionary<string, string> result, Dictionary<string, string> parameter, double score)
            {
                this.result = result;
                this.parameter = parameter;
                this.score = score;
            }
        }

        private void AnalyseRawTestDataForm_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Config.startupPath;
            while (openFileDialog1.ShowDialog() != DialogResult.OK) ;
            string all = File.ReadAllText(openFileDialog1.FileName);
            string[] lines = all.Split(Environment.NewLine.ToCharArray());
            foreach(string line in lines)
            {
                if (line != null && line != "")
                {
                    string[] colums = line.Split('@');

                    Dictionary<string, string> result = BacktestFormatter.convertStringCodedToParameters(colums[0]);
                    Dictionary<string, string> parameter = BacktestFormatter.convertStringCodedToParameters(colums[1]);

                    double score = -1;
                    if (isGoodResult(result, parameter, ref score))
                    {
                        data.Add(new SimpleStrategyResult(result, parameter, score));
                    }
                }
            }

            data = data.OrderBy(x => x.score).ToList();
            data.Reverse();

            List<string> foundPairs = new List<string>();

            foreach (SimpleStrategyResult result in data)
            {
                if (foundPairs.Contains(result.parameter["pair"]) == false)
                {
                    if (onlyParameters)
                        textBox1.Text += BacktestFormatter.getDictStringCoded(result.parameter) + Environment.NewLine;
                    else
                        textBox1.Text += Math.Round(result.score, 5) + "\t" + BacktestFormatter.getDictStringCoded(result.parameter) + Environment.NewLine + BacktestFormatter.getDictStringCoded(result.result) + Environment.NewLine + Environment.NewLine;

                    foundPairs.Add(result.parameter["pair"]);
                }
            }
        }

        private bool isGoodResult(Dictionary<string, string> result, Dictionary<string, string> parameter, ref double score)
        {
            double profit = Double.Parse(result["profit"]);
            double drawdown = Double.Parse(result["drawdown"]);
            double winratio = Double.Parse(result["winratio"]);

            double meanWin = Double.Parse(result["meanwin"]);
            double meanLoss = Double.Parse(result["meanloss"]);

            double deviationWin = Double.Parse(result["stddeviationwin"]);

            if (parameter.ContainsKey("sl") && parameter.ContainsKey("tp"))
            {
                double sl = Double.Parse(parameter["sl"]);
                double tp = Double.Parse(parameter["tp"]);

                if (sl > tp)
                    return false;
            }

            double expectedReturn = (meanWin * winratio) - Math.Abs(meanLoss * (1 - winratio));

            score = expectedReturn / (1 + deviationWin); //Devided by the devation of win?

            return profit > 0
                && Convert.ToInt16(result["positions"]) >= 30
                && profit >= Math.Abs(drawdown * 4)
                && expectedReturn >= 0;
        }
    }
}
