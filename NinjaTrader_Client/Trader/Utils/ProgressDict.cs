using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Utils
{
    public class ProgressDict
    {
        private Dictionary<string, int> progress = new Dictionary<string, int>();
        public void setProgress(string name, int percent)
        {
            if (progress.ContainsKey(name) == false)
                progress.Add(name, percent);
            else
                progress[name] = percent;
        }

        string oldOutput = "";
        public string getString()
        {
            int all = 0;
            try {
                string outputStr = "";
                foreach (KeyValuePair<string, int> pair in progress)
                {
                    outputStr += pair.Key + ": " + pair.Value + Environment.NewLine;
                    all += pair.Value;
                }

                if (outputStr == "")
                    outputStr = "Nothing to show";

                if(progress.Count != 0)
                    outputStr += "Total: " + (all / progress.Count);

                oldOutput = outputStr;

                return outputStr;
            }
            catch { return oldOutput; }
        }

        public void remove(string name)
        {
            if (progress.ContainsKey(name))
                progress.Remove(name);
        }
    }
}
