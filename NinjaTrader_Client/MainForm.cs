using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NinjaTrader.Client;
using NinjaTrader_Client.Trader;
using NinjaTrader_Client.Trader.Model;

namespace NinjaTrader_Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Main main;
        private void Form1_Load(object sender, EventArgs e)
        {
            main = new Main();
            main.uiDataChanged += updateUI;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.stop();
        }

        public void updateUI(UIData data)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => updateUI(data)));
                return;
            }

            label1.Text = "Errors: " + data.dbErrors;
            label2.Text = "Datasets: " + data.dataSets;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            main.startDownloadingUpdates();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartingForm cf = new ChartingForm(main.getDatabase());
            cf.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Wirklich migrieren?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                main.getDatabase().megrate();
        }
    }
}
