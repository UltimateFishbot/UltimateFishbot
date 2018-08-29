namespace UltimateFishBot.Forms
{
    partial class frmDirections
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDirections));
            this.Button1 = new System.Windows.Forms.Button();
            this.Label3Desc = new System.Windows.Forms.Label();
            this.Label3Title = new System.Windows.Forms.Label();
            this.Label2Desc = new System.Windows.Forms.Label();
            this.Label2Title = new System.Windows.Forms.Label();
            this.Label1Desc = new System.Windows.Forms.Label();
            this.Label1Title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button1
            // 
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button1.Location = new System.Drawing.Point(193, 231);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Ok";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Label3Desc
            // 
            this.Label3Desc.AutoSize = true;
            this.Label3Desc.Location = new System.Drawing.Point(42, 202);
            this.Label3Desc.Name = "Label3Desc";
            this.Label3Desc.Size = new System.Drawing.Size(330, 26);
            this.Label3Desc.TabIndex = 12;
            this.Label3Desc.Text = "* Press Start, sit back, and let The Gorton\'s Fisherman handle things.\r\n\r\n";
            // 
            // Label3Title
            // 
            this.Label3Title.AutoSize = true;
            this.Label3Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3Title.Location = new System.Drawing.Point(9, 185);
            this.Label3Title.Name = "Label3Title";
            this.Label3Title.Size = new System.Drawing.Size(57, 13);
            this.Label3Title.TabIndex = 11;
            this.Label3Title.Text = "3.)  Fish!";
            // 
            // Label2Desc
            // 
            this.Label2Desc.AutoSize = true;
            this.Label2Desc.Location = new System.Drawing.Point(42, 114);
            this.Label2Desc.Name = "Label2Desc";
            this.Label2Desc.Size = new System.Drawing.Size(291, 52);
            this.Label2Desc.TabIndex = 10;
            this.Label2Desc.Text = "* Walk to some lake/water\r\n* Zoom in as much as possible\r\n* Find a location with " +
    "as little sound as possible\r\n* Avoid locations with anything that changes the mo" +
    "use icon\r\n";
            // 
            // Label2Title
            // 
            this.Label2Title.AutoSize = true;
            this.Label2Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2Title.Location = new System.Drawing.Point(9, 97);
            this.Label2Title.Name = "Label2Title";
            this.Label2Title.Size = new System.Drawing.Size(118, 13);
            this.Label2Title.TabIndex = 9;
            this.Label2Title.Text = "2.)  Find a Location";
            // 
            // Label1Desc
            // 
            this.Label1Desc.AutoSize = true;
            this.Label1Desc.Location = new System.Drawing.Point(42, 22);
            this.Label1Desc.Name = "Label1Desc";
            this.Label1Desc.Size = new System.Drawing.Size(396, 52);
            this.Label1Desc.TabIndex = 8;
            this.Label1Desc.Text = resources.GetString("Label1Desc.Text");
            // 
            // Label1Title
            // 
            this.Label1Title.AutoSize = true;
            this.Label1Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1Title.Location = new System.Drawing.Point(9, 9);
            this.Label1Title.Name = "Label1Title";
            this.Label1Title.Size = new System.Drawing.Size(125, 13);
            this.Label1Title.TabIndex = 7;
            this.Label1Title.Text = "1.)  Setting Up WoW";
            // 
            // frmDirections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 264);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.Label3Desc);
            this.Controls.Add(this.Label3Title);
            this.Controls.Add(this.Label2Desc);
            this.Controls.Add(this.Label2Title);
            this.Controls.Add(this.Label1Desc);
            this.Controls.Add(this.Label1Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmDirections";
            this.Text = "Directions";
            this.Load += new System.EventHandler(this.frmDirections_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.Label Label3Desc;
        internal System.Windows.Forms.Label Label3Title;
        internal System.Windows.Forms.Label Label2Desc;
        internal System.Windows.Forms.Label Label2Title;
        internal System.Windows.Forms.Label Label1Desc;
        internal System.Windows.Forms.Label Label1Title;
    }
}