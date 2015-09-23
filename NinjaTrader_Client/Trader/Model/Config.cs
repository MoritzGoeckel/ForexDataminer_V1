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
        public string mongodbExePath;
        public string mongodbDataPath;

        public Config(string startupPath)
        {
            if (File.Exists(startupPath + "/config.json"))
            {
                string json = File.ReadAllText(startupPath + "/config.json");
                dynamic config = JObject.Parse(json);

                mongodbExePath = config.mongodbExePath;
                mongodbDataPath = config.mogodbDataPath;
            }
            else
            {
                //ask for paths

                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "mongod.exe";
                fileDialog.InitialDirectory = startupPath;

                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                //Data

                while (true)
                {
                    if (fileDialog.ShowDialog() == DialogResult.OK && folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        JObject config = new JObject();
                        config["mongodbExePath"] = fileDialog.FileName;
                        config["mogodbDataPath"] = folderDialog.SelectedPath;
                        File.WriteAllText(startupPath + "/config.json", config.ToString());

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
