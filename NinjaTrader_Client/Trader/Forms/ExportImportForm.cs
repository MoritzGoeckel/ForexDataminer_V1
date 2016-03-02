using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client
{
    public partial class ExportImportForm : Form
    {
        private Database database;
        public ExportImportForm(Database database)
        {
            InitializeComponent();
            this.database = database;
        }

        private void ExportImportForm_Load(object sender, EventArgs e)
        {
            textBox_now.Text = database.getLastTimestamp().ToString();
        }

        private void export_btn_Click(object sender, EventArgs e)
        {
            if(textBox_from.Text == "")
            {
                MessageBox.Show("Please enter a timestart");
                return;
            }
            else
            {
                export_btn.Enabled = false;

                Thread exportThread = new Thread(export);
                exportThread.Start();
            }
        }

        private void reportResult(string msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => reportResult(msg)));
                return;
            }

            export_btn.Enabled = true;
            MessageBox.Show(msg);
        }

        private void export()
        {
            /*try
            {
                database.exportData(Convert.ToInt64(textBox_from.Text), Application.StartupPath);
                reportResult("Export successful");
            }
            catch (Exception ex)
            {
                reportResult("Export failed: " + ex.Message);
            }*/
        }

        private void import_btn_Click(object sender, EventArgs e)
        {
            /*OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Application.StartupPath;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fileDialog.FileName.Substring(0, fileDialog.FileName.LastIndexOf('\\'));
                string file = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf('\\') + 1);

                DirectoryInfo dir = new DirectoryInfo(path);
                foreach(FileInfo fileinfo in dir.GetFiles(file.Substring(0, file.LastIndexOf("PART")) + "*"))
                {
                    string content = File.ReadAllText(fileinfo.FullName);
                    content = StringCompressor.DecompressString(content);

                    database.importData(content);
                }

                textBox_now.Text = database.getLastTimestamp().ToString();
                MessageBox.Show("Import done.");
            }
            else
                MessageBox.Show("Please select a file to import");*/
        }
    }
}
