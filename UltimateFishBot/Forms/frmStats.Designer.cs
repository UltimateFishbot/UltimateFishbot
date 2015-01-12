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
            this.labelSuccess = new System.Windows.Forms.Label();
            this.labelMissedCount = new System.Windows.Forms.Label();
            this.labelMissed = new System.Windows.Forms.Label();
            this.labelSuccessCount = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelTotalCount = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.timerUpdateStats = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labelSuccess
            // 
            this.labelSuccess.AutoSize = true;
            this.labelSuccess.Location = new System.Drawing.Point(13, 13);
            this.labelSuccess.Name = "labelSuccess";
            this.labelSuccess.Size = new System.Drawing.Size(101, 13);
            this.labelSuccess.TabIndex = 0;
            this.labelSuccess.Text = "Successful Fishing :";
            // 
            // labelMissedCount
            // 
            this.labelMissedCount.AutoSize = true;
            this.labelMissedCount.Location = new System.Drawing.Point(141, 30);
            this.labelMissedCount.Name = "labelMissedCount";
            this.labelMissedCount.Size = new System.Drawing.Size(13, 13);
            this.labelMissedCount.TabIndex = 1;
            this.labelMissedCount.Text = "0";
            // 
            // labelMissed
            // 
            this.labelMissed.AutoSize = true;
            this.labelMissed.Location = new System.Drawing.Point(13, 30);
            this.labelMissed.Name = "labelMissed";
            this.labelMissed.Size = new System.Drawing.Size(68, 13);
            this.labelMissed.TabIndex = 2;
            this.labelMissed.Text = "Missed Fish :";
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
            this.buttonClose.Location = new System.Drawing.Point(87, 66);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(13, 47);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(37, 13);
            this.labelTotal.TabIndex = 5;
            this.labelTotal.Text = "Total :";
            // 
            // labelTotalCount
            // 
            this.labelTotalCount.AutoSize = true;
            this.labelTotalCount.Location = new System.Drawing.Point(141, 47);
            this.labelTotalCount.Name = "labelTotalCount";
            this.labelTotalCount.Size = new System.Drawing.Size(13, 13);
            this.labelTotalCount.TabIndex = 6;
            this.labelTotalCount.Text = "0";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(6, 66);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 7;
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
            // frmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(166, 101);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.labelTotalCount);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelSuccessCount);
            this.Controls.Add(this.labelMissed);
            this.Controls.Add(this.labelMissedCount);
            this.Controls.Add(this.labelSuccess);
            this.Name = "frmStats";
            this.Text = "Statistics";
            this.Load += new System.EventHandler(this.frmStats_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSuccess;
        private System.Windows.Forms.Label labelMissedCount;
        private System.Windows.Forms.Label labelMissed;
        private System.Windows.Forms.Label labelSuccessCount;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelTotalCount;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Timer timerUpdateStats;
    }
}