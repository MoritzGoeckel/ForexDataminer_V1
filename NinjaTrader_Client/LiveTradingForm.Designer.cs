namespace NinjaTrader_Client
{
    partial class LiveTradingForm
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
            this.components = new System.ComponentModel.Container();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.labelInfo = new System.Windows.Forms.Label();
            this.goLongBtn = new System.Windows.Forms.Button();
            this.goShortBtn = new System.Windows.Forms.Button();
            this.closeLongBtn = new System.Windows.Forms.Button();
            this.closeShortBtn = new System.Windows.Forms.Button();
            this.closeAllBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(35, 13);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "label1";
            // 
            // goLongBtn
            // 
            this.goLongBtn.Location = new System.Drawing.Point(15, 195);
            this.goLongBtn.Name = "goLongBtn";
            this.goLongBtn.Size = new System.Drawing.Size(75, 23);
            this.goLongBtn.TabIndex = 1;
            this.goLongBtn.Text = "Long";
            this.goLongBtn.UseVisualStyleBackColor = true;
            this.goLongBtn.Click += new System.EventHandler(this.goLongBtn_Click);
            // 
            // goShortBtn
            // 
            this.goShortBtn.Location = new System.Drawing.Point(197, 195);
            this.goShortBtn.Name = "goShortBtn";
            this.goShortBtn.Size = new System.Drawing.Size(75, 23);
            this.goShortBtn.TabIndex = 2;
            this.goShortBtn.Text = "Short";
            this.goShortBtn.UseVisualStyleBackColor = true;
            this.goShortBtn.Click += new System.EventHandler(this.goShortBtn_Click);
            // 
            // closeLongBtn
            // 
            this.closeLongBtn.Location = new System.Drawing.Point(15, 224);
            this.closeLongBtn.Name = "closeLongBtn";
            this.closeLongBtn.Size = new System.Drawing.Size(75, 23);
            this.closeLongBtn.TabIndex = 3;
            this.closeLongBtn.Text = "Close";
            this.closeLongBtn.UseVisualStyleBackColor = true;
            this.closeLongBtn.Click += new System.EventHandler(this.closeLongBtn_Click);
            // 
            // closeShortBtn
            // 
            this.closeShortBtn.Location = new System.Drawing.Point(197, 224);
            this.closeShortBtn.Name = "closeShortBtn";
            this.closeShortBtn.Size = new System.Drawing.Size(75, 23);
            this.closeShortBtn.TabIndex = 4;
            this.closeShortBtn.Text = "Close";
            this.closeShortBtn.UseVisualStyleBackColor = true;
            this.closeShortBtn.Click += new System.EventHandler(this.closeShortBtn_Click);
            // 
            // closeAllBtn
            // 
            this.closeAllBtn.Location = new System.Drawing.Point(106, 224);
            this.closeAllBtn.Name = "closeAllBtn";
            this.closeAllBtn.Size = new System.Drawing.Size(75, 23);
            this.closeAllBtn.TabIndex = 5;
            this.closeAllBtn.Text = "Close All";
            this.closeAllBtn.UseVisualStyleBackColor = true;
            this.closeAllBtn.Click += new System.EventHandler(this.closeAllBtn_Click);
            // 
            // LiveTradingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.closeAllBtn);
            this.Controls.Add(this.closeShortBtn);
            this.Controls.Add(this.closeLongBtn);
            this.Controls.Add(this.goShortBtn);
            this.Controls.Add(this.goLongBtn);
            this.Controls.Add(this.labelInfo);
            this.Name = "LiveTradingForm";
            this.Text = "LiveTradingForm";
            this.Load += new System.EventHandler(this.LiveTradingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button goLongBtn;
        private System.Windows.Forms.Button goShortBtn;
        private System.Windows.Forms.Button closeLongBtn;
        private System.Windows.Forms.Button closeShortBtn;
        private System.Windows.Forms.Button closeAllBtn;
    }
}