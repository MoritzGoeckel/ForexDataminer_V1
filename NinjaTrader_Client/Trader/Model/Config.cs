using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader.Model
{
    class Config
    {
        public static string errorLogPath;
        public static string startupPath;
        public static string sqlitePath;

        public static void startConfig(string startupPath)
        {
            Config.startupPath = startupPath;

            if (File.Exists(startupPath + "\\config.json"))
            {
                string json = File.ReadAllText(startupPath + "\\config.json");
                dynamic config = JObject.Parse(json);

                sqlitePath = config.sqlitePath;
                errorLogPath = config.errorLogPath;
            }
            else
            {
                JObject config = new JObject();
                config["errorLogPath"] = startupPath + "\\error.log";
                config["sqlitePath"] = startupPath + "\\priceHistorySQLite.s3db";

                File.WriteAllText(startupPath + "\\config.json", config.ToString());

                startConfig(startupPath);
            }
        }
    }
}
