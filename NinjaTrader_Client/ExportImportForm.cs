using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NinjaTrader_Client.Trader;
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
            try
            {
                BsonDocument export = database.getExportData(Convert.ToInt64(textBox_from.Text));

                if (Directory.Exists(Application.StartupPath + "/export") == false)
                    Directory.CreateDirectory(Application.StartupPath + "/export");

                string export_str = export.ToString();
                export_str = StringCompressor.CompressString(export_str);

                File.WriteAllText(Application.StartupPath + "/export/database_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + textBox_from.Text + "_" + Timestamp.getNow().ToString() + ".json", export_str);

                reportResult("Export successful");
            }
            catch (Exception ex)
            {
                reportResult("Export failed: " + ex.Message);
            }
        }

        private void import_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Application.StartupPath;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string content = File.ReadAllText(fileDialog.FileName);
                content = StringCompressor.DecompressString(content);

                database.importData(content);
                textBox_now.Text = database.getLastTimestamp().ToString();
                MessageBox.Show("Import done");
            }
            else
                MessageBox.Show("Please select a file to import");
        }
    }
}
