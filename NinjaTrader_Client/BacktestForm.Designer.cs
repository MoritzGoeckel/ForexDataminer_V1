namespace NinjaTrader_Client
{
    partial class BacktestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_info = new System.Windows.Forms.Label();
            this.label_trades = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_info.Location = new System.Drawing.Point(233, 12);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(45, 16);
            this.label_info.TabIndex = 1;
            this.label_info.Text = "label1";
            // 
            // label_trades
            // 
            this.label_trades.AutoSize = true;
            this.label_trades.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_trades.Location = new System.Drawing.Point(12, 12);
            this.label_trades.Name = "label_trades";
            this.label_trades.Size = new System.Drawing.Size(45, 16);
            this.label_trades.TabIndex = 2;
            this.label_trades.Text = "label1";
            // 
            // BacktestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 521);
            this.Controls.Add(this.label_trades);
            this.Controls.Add(this.label_info);
            this.Name = "BacktestForm";
            this.Text = "BacktestForm";
            this.Load += new System.EventHandler(this.BacktestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.Label label_trades;
    }
}