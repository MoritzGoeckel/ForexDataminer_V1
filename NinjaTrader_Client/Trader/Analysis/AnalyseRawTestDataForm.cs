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
        public AnalyseRawTestDataForm()
        {
            InitializeComponent();
        }

        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> parameters = new List<Dictionary<string, string>>();

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

                    if (isGoodResult(result))
                    {
                        results.Add(result);
                        parameters.Add(BacktestFormatter.convertStringCodedToParameters(colums[1]));
                    }
                }
            }

            for(int i = 0; i < results.Count; i++)
            {
                textBox1.Text += BacktestFormatter.getDictStringCoded(parameters[i]) + Environment.NewLine + BacktestFormatter.getDictStringCoded(results[i]) + Environment.NewLine + Environment.NewLine;
            }
        }

        private bool isGoodResult(Dictionary<string, string> result)
        {
            double profit = Double.Parse(result["profit"]);
            double drawdown = Double.Parse(result["drawdown"]);

            return Convert.ToInt16(result["positions"]) >= 30 && Double.Parse(result["winratio"]) >= 0.6 && profit >= drawdown * 5;
        }
    }
}
