using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NinjaTrader_Client.Trader
{
    public class SSI_Downloader
    {
        private List<string> instruments = new List<string>();
        private Dictionary<string, double> old_ssi_values = new Dictionary<string,double>();
        private Dictionary<string, double> old_ssi_win_values = new Dictionary<string, double>();

        private Thread timerThread;

        private int interval = 1 * 60 * 1000;

        public int errors = 0;

        public delegate void SourceDataArrivedHandler(double value, long timestamp, string sourceName, string instrument);
        public event SourceDataArrivedHandler sourceDataArrived;

        public SSI_Downloader(List<string> instruments)
        {
            this.instruments.AddRange(instruments);

            foreach (string name in instruments)
            {
                this.old_ssi_values.Add(name, 99999999);
                this.old_ssi_win_values.Add(name, 9999999);
            }

            timerThread = new Thread(timedFunction);
        }

        public void start()
        {
            timerThread.Start();
        }

        bool resume = true;
        private void timedFunction()
        {
            WebClient webClient = new WebClient();
            while(resume)
            {
                foreach (string instrument in instruments)
                {
                    try
                    {
                        string data = webClient.DownloadString("http://www.fxblue.com/woc/_WocCurrentFeed.aspx?s=" + instrument + "&rx=1442515612434");
                        data = data.Replace("symbol", "\"symbol\"");
                        data = data.Replace("isUsable", "\"isUsable\"");
                        data = data.Replace("longWin", "\"longWin\"");
                        data = data.Replace("longLoss", "\"longLoss\"");
                        data = data.Replace("shortWin", "\"shortWin\"");
                        data = data.Replace("shortLoss", "\"shortLoss\"");
                        data = data.Replace("percentOfTotalTraders", "\"percentOfTotalTraders\"");

                        data = data.Substring(1, data.Length - 2);

                        dynamic json = JObject.Parse(data);

                        int inLongPosition = json.longWin + json.longLoss;
                        double ssi_value = ((((double)inLongPosition) / 100d) - 0.5d) * 2; //Über 0 -> mehr long, Unter 0 -> Mehr short || -1 >> ssi_win_value >> 1

                        double longWinRatio = (double)json.longWin / ((double)json.longLoss + (double)json.longWin);
                        double shortWinRatio = (double)json.shortWin / ((double)json.shortLoss + (double)json.shortWin);

                        double ssi_win_value = longWinRatio - shortWinRatio; //Über 0 -> Long gewinnt, Unter 0 -> Short gewinnt || -1 >> ssi_win_value >> 1

                        //SSI
                        if (old_ssi_values[instrument] != ssi_value && sourceDataArrived != null)
                            sourceDataArrived(ssi_value, Timestamp.getNow(), "ssi-mt4", instrument);

                        old_ssi_values[instrument] = ssi_value;

                        //SSI-Win
                        if (old_ssi_win_values[instrument] != ssi_win_value && sourceDataArrived != null)
                            sourceDataArrived(ssi_win_value, Timestamp.getNow(), "ssi-win-mt4", instrument);

                        old_ssi_values[instrument] = ssi_win_value;
                    }
                    catch { errors++; }
                }

                Thread.Sleep(interval);
            }
        }

        public void stop()
        {
            resume = false;
            timerThread.Abort();
        }
    }
}
