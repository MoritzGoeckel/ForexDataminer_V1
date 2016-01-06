using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaTrader_Client.Trader
{
    public partial class ShowImageForm : Form
    {
        Image image;
        public ShowImageForm(Image image)
        {
            InitializeComponent();
            this.image = image;
        }

        private void ShowImageForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = image;
        }
    }
}
