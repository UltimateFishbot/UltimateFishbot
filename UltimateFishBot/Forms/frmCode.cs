using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes;

namespace UltimateFishBot.Forms
{
    public partial class frmCode : Form
    {
        public frmCode()
        {
            InitializeComponent();
        }

        private void frmCode_Load(object sender, EventArgs e)
        {
            this.Text           = Translate.GetTranslate("frmCode", "TITLE");
            LabelTitle.Text     = Translate.GetTranslate("frmCode", "LIBELLE_TITLE");
            LabelDesc.Text      = Translate.GetTranslate("frmCode", "LIBELLE_DESC", Properties.Settings.Default.Startup);
            ButtonEnter.Text    = Translate.GetTranslate("frmCode", "LIBELLE_ENTER");

            Process.Start("http://www.fishbot.net/code.html");
            this.Activate();
        }

        private void ButtonEnter_Click(object sender, EventArgs e)
        {
            System.DateTime myDate = System.DateTime.Now;
            string Expected = (Convert.ToInt32(myDate.DayOfWeek) * myDate.Day * (myDate.Month - 1) * 26).ToString();

            if (Expected.Length > 3)
                Expected = Expected.Substring(0, 3);

            if (TextBox1.Text == Expected)
                DialogResult = DialogResult.OK;
            else
                MessageBox.Show(Translate.GetTranslate("frmCode", "LIBELLE_TRY_AGAIN"));
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.fishbot.net/code.html");
        }
    }
}
