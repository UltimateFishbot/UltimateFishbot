using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes;

namespace UltimateFishBot.Forms
{
    public partial class frmStats : Form
    {
        public frmStats(Manager manager)
        {
            InitializeComponent();

            m_manager = manager;
            UpdateStats();
        }

        private void frmStats_Load(object sender, EventArgs e)
        {
            this.Text           = Translate.GetTranslate("frmStats", "TITLE");
            labelSuccess.Text   = Translate.GetTranslate("frmStats", "LABEL_SUCCESS");
            labelNotFound.Text  = Translate.GetTranslate("frmStats", "LABEL_NOT_FOUND");
            labelNotEared.Text  = Translate.GetTranslate("frmStats", "LABEL_NOT_EARED");
            labelTotal.Text     = Translate.GetTranslate("frmStats", "LABEL_TOTAL");
            buttonReset.Text    = Translate.GetTranslate("frmStats", "BUTTON_RESET");
            buttonClose.Text    = Translate.GetTranslate("frmStats", "BUTTON_CLOSE");
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            labelSuccessCount.Text  = "0";
            labelNotFoundCount.Text = "0";
            labelNotEaredCount.Text = "0";
            labelTotalCount.Text    = "0";

            m_manager.ResetFishingStats();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timerUpdateStats_Tick(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void UpdateStats()
        {
            Manager.FishingStats stats  = m_manager.GetFishingStats();
            labelSuccessCount.Text      = stats.totalSuccessFishing.ToString();
            labelNotFoundCount.Text     = stats.totalNotFoundFish.ToString();
            labelNotEaredCount.Text     = stats.totalNotEaredFish.ToString();
            labelTotalCount.Text        = stats.Total().ToString();
        }

        Manager m_manager;
    }
}
