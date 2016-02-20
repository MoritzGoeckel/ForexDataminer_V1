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
        private Dictionary<string, string> progressString = new Dictionary<string, string>();


        public void setProgress(string name, int percent)
        {
            if (progress.ContainsKey(name) == false)
                progress.Add(name, percent);
            else
                progress[name] = percent;
        }

        public void setProgress(string name, string state)
        {
            if (progressString.ContainsKey(name) == false)
                progressString.Add(name, state);
            else
                progressString[name] = state;
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

                foreach (KeyValuePair<string, string> pair in progressString)
                {
                    outputStr += pair.Key + ": " + pair.Value + Environment.NewLine;
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

            if (progressString.ContainsKey(name))
                progressString.Remove(name);
        }
    }
}
