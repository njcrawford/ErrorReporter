namespace ReportTester
{
    partial class Form1
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
            this.btnCrash = new System.Windows.Forms.Button();
            this.btnManualReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCrash
            // 
            this.btnCrash.Location = new System.Drawing.Point(12, 101);
            this.btnCrash.Name = "btnCrash";
            this.btnCrash.Size = new System.Drawing.Size(260, 23);
            this.btnCrash.TabIndex = 0;
            this.btnCrash.Text = "Click here to cause a divide-by-zero exception";
            this.btnCrash.UseVisualStyleBackColor = true;
            this.btnCrash.Click += new System.EventHandler(this.btnCrash_Click);
            // 
            // btnManualReport
            // 
            this.btnManualReport.Location = new System.Drawing.Point(12, 165);
            this.btnManualReport.Name = "btnManualReport";
            this.btnManualReport.Size = new System.Drawing.Size(260, 23);
            this.btnManualReport.TabIndex = 1;
            this.btnManualReport.Text = "Click here to manually report an error";
            this.btnManualReport.UseVisualStyleBackColor = true;
            this.btnManualReport.Click += new System.EventHandler(this.btnManualReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnManualReport);
            this.Controls.Add(this.btnCrash);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCrash;
        private System.Windows.Forms.Button btnManualReport;
    }
}

