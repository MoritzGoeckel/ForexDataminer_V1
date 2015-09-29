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
            this.label_parameters = new System.Windows.Forms.Label();
            this.label_trades = new System.Windows.Forms.Label();
            this.label_result = new System.Windows.Forms.Label();
            this.listBox_results = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label_parameters
            // 
            this.label_parameters.AutoSize = true;
            this.label_parameters.Location = new System.Drawing.Point(211, 9);
            this.label_parameters.Name = "label_parameters";
            this.label_parameters.Size = new System.Drawing.Size(87, 13);
            this.label_parameters.TabIndex = 0;
            this.label_parameters.Text = "label_parameters";
            // 
            // label_trades
            // 
            this.label_trades.AutoSize = true;
            this.label_trades.Location = new System.Drawing.Point(411, 9);
            this.label_trades.Name = "label_trades";
            this.label_trades.Size = new System.Drawing.Size(64, 13);
            this.label_trades.TabIndex = 1;
            this.label_trades.Text = "label_trades";
            // 
            // label_result
            // 
            this.label_result.AutoSize = true;
            this.label_result.Location = new System.Drawing.Point(515, 9);
            this.label_result.Name = "label_result";
            this.label_result.Size = new System.Drawing.Size(60, 13);
            this.label_result.TabIndex = 2;
            this.label_result.Text = "label_result";
            // 
            // listBox_results
            // 
            this.listBox_results.FormattingEnabled = true;
            this.listBox_results.Location = new System.Drawing.Point(12, 9);
            this.listBox_results.Name = "listBox_results";
            this.listBox_results.Size = new System.Drawing.Size(176, 498);
            this.listBox_results.TabIndex = 3;
            this.listBox_results.SelectedIndexChanged += new System.EventHandler(this.listBox_results_SelectedIndexChanged_1);
            // 
            // BacktestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 521);
            this.Controls.Add(this.listBox_results);
            this.Controls.Add(this.label_result);
            this.Controls.Add(this.label_trades);
            this.Controls.Add(this.label_parameters);
            this.Name = "BacktestForm";
            this.Text = "BacktestForm";
            this.Load += new System.EventHandler(this.BacktestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_parameters;
        private System.Windows.Forms.Label label_trades;
        private System.Windows.Forms.Label label_result;
        private System.Windows.Forms.ListBox listBox_results;

    }
}