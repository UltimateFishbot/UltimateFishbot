namespace UltimateFishBot.Forms
{
    partial class frmStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStats));
            this.labelSuccess = new System.Windows.Forms.Label();
            this.labelNotFoundCount = new System.Windows.Forms.Label();
            this.labelNotFound = new System.Windows.Forms.Label();
            this.labelSuccessCount = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelNotEared = new System.Windows.Forms.Label();
            this.labelNotEaredCount = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.timerUpdateStats = new System.Windows.Forms.Timer(this.components);
            this.labelTotalCount = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelSuccess
            // 
            this.labelSuccess.AutoSize = true;
            this.labelSuccess.Location = new System.Drawing.Point(13, 13);
            this.labelSuccess.Name = "labelSuccess";
            this.labelSuccess.Size = new System.Drawing.Size(101, 13);
            this.labelSuccess.TabIndex = 50;
            this.labelSuccess.Text = "Successful Fishing :";
            // 
            // labelNotFoundCount
            // 
            this.labelNotFoundCount.AutoSize = true;
            this.labelNotFoundCount.Location = new System.Drawing.Point(141, 30);
            this.labelNotFoundCount.Name = "labelNotFoundCount";
            this.labelNotFoundCount.Size = new System.Drawing.Size(13, 13);
            this.labelNotFoundCount.TabIndex = 1;
            this.labelNotFoundCount.Text = "0";
            // 
            // labelNotFound
            // 
            this.labelNotFound.AutoSize = true;
            this.labelNotFound.Location = new System.Drawing.Point(13, 30);
            this.labelNotFound.Name = "labelNotFound";
            this.labelNotFound.Size = new System.Drawing.Size(80, 13);
            this.labelNotFound.TabIndex = 2;
            this.labelNotFound.Text = "Fish not found :";
            // 
            // labelSuccessCount
            // 
            this.labelSuccessCount.AutoSize = true;
            this.labelSuccessCount.Location = new System.Drawing.Point(141, 13);
            this.labelSuccessCount.Name = "labelSuccessCount";
            this.labelSuccessCount.Size = new System.Drawing.Size(13, 13);
            this.labelSuccessCount.TabIndex = 3;
            this.labelSuccessCount.Text = "0";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(89, 81);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelNotEared
            // 
            this.labelNotEared.AutoSize = true;
            this.labelNotEared.Location = new System.Drawing.Point(13, 47);
            this.labelNotEared.Name = "labelNotEared";
            this.labelNotEared.Size = new System.Drawing.Size(80, 13);
            this.labelNotEared.TabIndex = 5;
            this.labelNotEared.Text = "Fish not eared :";
            // 
            // labelNotEaredCount
            // 
            this.labelNotEaredCount.AutoSize = true;
            this.labelNotEaredCount.Location = new System.Drawing.Point(141, 47);
            this.labelNotEaredCount.Name = "labelNotEaredCount";
            this.labelNotEaredCount.Size = new System.Drawing.Size(13, 13);
            this.labelNotEaredCount.TabIndex = 6;
            this.labelNotEaredCount.Text = "0";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(8, 81);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 1;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // timerUpdateStats
            // 
            this.timerUpdateStats.Enabled = true;
            this.timerUpdateStats.Interval = 2000;
            this.timerUpdateStats.Tick += new System.EventHandler(this.timerUpdateStats_Tick);
            // 
            // labelTotalCount
            // 
            this.labelTotalCount.AutoSize = true;
            this.labelTotalCount.Location = new System.Drawing.Point(141, 64);
            this.labelTotalCount.Name = "labelTotalCount";
            this.labelTotalCount.Size = new System.Drawing.Size(13, 13);
            this.labelTotalCount.TabIndex = 9;
            this.labelTotalCount.Text = "0";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(13, 64);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(37, 13);
            this.labelTotal.TabIndex = 8;
            this.labelTotal.Text = "Total :";
            // 
            // frmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(171, 110);
            this.Controls.Add(this.labelTotalCount);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.labelNotEaredCount);
            this.Controls.Add(this.labelNotEared);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelSuccessCount);
            this.Controls.Add(this.labelNotFound);
            this.Controls.Add(this.labelNotFoundCount);
            this.Controls.Add(this.labelSuccess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmStats";
            this.Text = "Statistics";
            this.Load += new System.EventHandler(this.frmStats_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSuccess;
        private System.Windows.Forms.Label labelNotFoundCount;
        private System.Windows.Forms.Label labelNotFound;
        private System.Windows.Forms.Label labelSuccessCount;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelNotEared;
        private System.Windows.Forms.Label labelNotEaredCount;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Timer timerUpdateStats;
        private System.Windows.Forms.Label labelTotalCount;
        private System.Windows.Forms.Label labelTotal;
    }
}