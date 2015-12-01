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
        public static string mongodbExePath;
        public static string mongodbDataPath;
        public static string errorLogPath;
        public static string startupPath;

        public static void startConfig(string startupPath)
        {
            Config.startupPath = startupPath;

            if (File.Exists(startupPath + "\\config.json"))
            {
                string json = File.ReadAllText(startupPath + "\\config.json");
                dynamic config = JObject.Parse(json);

                mongodbExePath = config.mongodbExePath;
                mongodbDataPath = config.mogodbDataPath;
                errorLogPath = config.errorLogPath;
            }
            else
            {
                //ask for paths

                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "mongod.exe";
                fileDialog.InitialDirectory = startupPath;

                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select the data-directory of the mongodb";

                //Data
                while (true)
                {
                    if (fileDialog.ShowDialog() == DialogResult.OK && folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        JObject config = new JObject();
                        config["mongodbExePath"] = fileDialog.FileName;
                        config["mogodbDataPath"] = folderDialog.SelectedPath;
                        config["errorLogPath"] = startupPath + "\\error.log";

                        File.WriteAllText(startupPath + "\\config.json", config.ToString());

                        mongodbExePath = config["mongodbExePath"].ToString();
                        mongodbDataPath = config["mogodbDataPath"].ToString();

                        break;
                    }
                    else
                        MessageBox.Show("Bitte beide Pfade angeben");
                }
            }
        }
    }
}
