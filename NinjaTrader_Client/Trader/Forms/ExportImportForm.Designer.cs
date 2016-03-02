namespace NinjaTrader_Client
{
    partial class ExportImportForm
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
            this.export_btn = new System.Windows.Forms.Button();
            this.import_btn = new System.Windows.Forms.Button();
            this.textBox_now = new System.Windows.Forms.TextBox();
            this.textBox_from = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // export_btn
            // 
            this.export_btn.Location = new System.Drawing.Point(12, 227);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(75, 23);
            this.export_btn.TabIndex = 0;
            this.export_btn.Text = "Export";
            this.export_btn.UseVisualStyleBackColor = true;
            this.export_btn.Click += new System.EventHandler(this.export_btn_Click);
            // 
            // import_btn
            // 
            this.import_btn.Location = new System.Drawing.Point(197, 227);
            this.import_btn.Name = "import_btn";
            this.import_btn.Size = new System.Drawing.Size(75, 23);
            this.import_btn.TabIndex = 1;
            this.import_btn.Text = "Import";
            this.import_btn.UseVisualStyleBackColor = true;
            this.import_btn.Click += new System.EventHandler(this.import_btn_Click);
            // 
            // textBox_now
            // 
            this.textBox_now.Location = new System.Drawing.Point(12, 147);
            this.textBox_now.Name = "textBox_now";
            this.textBox_now.Size = new System.Drawing.Size(100, 20);
            this.textBox_now.TabIndex = 2;
            // 
            // textBox_from
            // 
            this.textBox_from.Location = new System.Drawing.Point(172, 147);
            this.textBox_from.Name = "textBox_from";
            this.textBox_from.Size = new System.Drawing.Size(100, 20);
            this.textBox_from.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Last:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Export from:";
            // 
            // ExportImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_from);
            this.Controls.Add(this.textBox_now);
            this.Controls.Add(this.import_btn);
            this.Controls.Add(this.export_btn);
            this.Name = "ExportImportForm";
            this.Text = "ExportImportForm";
            this.Load += new System.EventHandler(this.ExportImportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button export_btn;
        private System.Windows.Forms.Button import_btn;
        private System.Windows.Forms.TextBox textBox_now;
        private System.Windows.Forms.TextBox textBox_from;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}