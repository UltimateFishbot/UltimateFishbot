using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes;
using UltimateFishBot.Classes.Helpers;
using UltimateFishBot.Forms;

namespace UltimateFishBot
{
    public partial class frmMain : Form
    {
        enum KeyModifier
        {
            None    = 0,
            Alt     = 1,
            Control = 2,
            Shift   = 4,
            WinKey  = 8
        }

        enum HotKey
        {
            StartStop   = 0
        }

        public frmMain()
        {
            InitializeComponent();

            m_manager = new Manager(this);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            btnStart.Text       = Translate.GetTranslate("frmMain", "BUTTON_START");
            btnStop.Text        = Translate.GetTranslate("frmMain", "BUTTON_STOP");
            btnSettings.Text    = Translate.GetTranslate("frmMain", "BUTTON_SETTINGS");
            btnStatistics.Text  = Translate.GetTranslate("frmMain", "BUTTON_STATISTICS");
            btnHowTo.Text       = Translate.GetTranslate("frmMain", "BUTTON_HTU");
            btnClose.Text       = Translate.GetTranslate("frmMain", "BUTTON_EXIT");
            lblStatus.Text      = Translate.GetTranslate("frmMain", "LABEL_STOPPED");

            Win32.RegisterHotKey(this.Handle, (int)HotKey.StartStop, (int)KeyModifier.Shift + (int)KeyModifier.Control, Keys.S.GetHashCode());
            CheckStatus();
        }

        private void CheckStatus()
        {
            lblWarn.Text = Translate.GetTranslate("frmMain", "LABEL_CHECKING_STATUS");
            lblWarn.Parent = PictureBox1;

            try
            {
                Task.Factory.StartNew(() => (new WebClient()).DownloadString("http://www.fishbot.net/status.txt"),
                    TaskCreationOptions.LongRunning).ContinueWith(x =>
                    {
                        if (x.Result.ToLower().Trim() != "safe")
                        {
                            lblWarn.Text = Translate.GetTranslate("frmMain", "LABEL_NO_LONGER_SAFE");
                            lblWarn.ForeColor = Color.Red;
                            lblWarn.BackColor = Color.Black;
                        }
                        else
                            lblWarn.Visible = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception)
            {
                lblWarn.Text = Translate.GetTranslate("frmMain", "LABEL_COULD_NOT_CHECK_STATUS");
            }                        
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!m_manager.IsStoppedOrPaused())
                return;

            if (m_manager.GetActualState() == Manager.FishingState.Stopped)
            {
                m_manager.Start();
                btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_START");
                btnStop.Text    = Translate.GetTranslate("frmMain", "BUTTON_PAUSE");
                lblStatus.Text  = Translate.GetTranslate("frmMain", "LABEL_STARTED");
            }
            else
            {
                m_manager.Resume();
                btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_RESUME");
                btnStop.Text    = Translate.GetTranslate("frmMain", "BUTTON_PAUSE");
                lblStatus.Text  = Translate.GetTranslate("frmMain", "LABEL_RESUMED");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (m_manager.GetActualState() == Manager.FishingState.Stopped)
                return;

            if (m_manager.GetActualState() == Manager.FishingState.Paused)
            {
                m_manager.Stop();
                btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_START");
                btnStop.Text    = Translate.GetTranslate("frmMain", "BUTTON_STOP");
                lblStatus.Text  = Translate.GetTranslate("frmMain", "LABEL_STOPPED");
            }
            else
            {
                m_manager.Pause();
                btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_RESUME");
                btnStop.Text    = Translate.GetTranslate("frmMain", "BUTTON_STOP");
                lblStatus.Text  = Translate.GetTranslate("frmMain", "LABEL_PAUSED");
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            new frmSettings().Show();
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {

            new frmStats(m_manager).Show();
        }

        private void btnHowTo_Click(object sender, EventArgs e)
        {
            new frmDirections().Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unregister all hotkeys before closing the form.
            foreach (HotKey hotKey in (HotKey[])Enum.GetValues(typeof(HotKey)))
                Win32.UnregisterHotKey(this.Handle, (int)hotKey);
        }

        public void StopFishing()
        {
            btnStop_Click(null, null);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                if (id == (int)HotKey.StartStop)
                {
                    if (m_manager.IsStoppedOrPaused())
                        btnStart_Click(null, null);
                    else
                        btnStop_Click(null, null);
                }
            }
        }

        private Manager m_manager;
        private static int WM_HOTKEY = 0x0312;
    }
}
