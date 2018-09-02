using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Helpers;
using UltimateFishBot.Properties;

namespace UltimateFishBot.Forms
{
    public partial class frmMain : Form, IManagerEventHandler
    {

        [Flags]
        private enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4
        }

        private enum HotKey
        {
            StartStop = 0,
            CursorCapture = 1
        }

        public frmMain()
        {
            ReloadHotkeys();
            InitializeComponent();

            _manager = new Manager(this, new Progress<string>(text =>
            {
                lblStatus.Text = text;
            }));
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            btnStart.Text      = Translate.GetTranslate("frmMain", "BUTTON_START");
            btnStop.Text       = Translate.GetTranslate("frmMain", "BUTTON_STOP");
            btnSettings.Text   = Translate.GetTranslate("frmMain", "BUTTON_SETTINGS");
            btnStatistics.Text = Translate.GetTranslate("frmMain", "BUTTON_STATISTICS");
            btnHowTo.Text      = Translate.GetTranslate("frmMain", "BUTTON_HTU");
            btnClose.Text      = Translate.GetTranslate("frmMain", "BUTTON_EXIT");
            btnAbout.Text      = Translate.GetTranslate("frmMain", "BUTTON_ABOUT");
            lblStatus.Text     = Translate.GetTranslate("frmMain", "LABEL_STOPPED");
            //this.Text          = "UltimateFishBot - v " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            /* Hide ? */
            
            ReloadHotkeys();
            await CheckStatus();
        }

        private async Task CheckStatus()
        {
            lblWarn.Text = Translate.GetTranslate("frmMain", "LABEL_CHECKING_STATUS");
            lblWarn.Parent = PictureBox1;

            try
            {
                string result = await (new WebClient().DownloadStringTaskAsync("http://www.robpaulson.com/fishbot/status.txt"));
                if (result.ToLower().Trim() != "safe")
                {
                    lblWarn.Text      = Translate.GetTranslate("frmMain", "LABEL_NO_LONGER_SAFE");
                    lblWarn.ForeColor = Color.Red;
                    lblWarn.BackColor = Color.Black;
                }
                else
                {
                    lblWarn.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblWarn.Text = Translate.GetTranslate("frmMain", "LABEL_COULD_NOT_CHECK_STATUS") + ex.Message;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            await _manager.StartOrResumeOrPause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _manager.Stop();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings.GetForm(this).Show();
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            frmStats.GetForm(_manager).Show();
        }

        private void btnHowTo_Click(object sender, EventArgs e)
        {
            frmDirections.GetForm.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY) {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                if (id == (int)HotKey.StartStop) {
                    Task.Factory.StartNew(async () => {
                        try {
                            await _manager.StartOrStop();
                        } catch (TaskCanceledException) {
                            // Do nothing, cancellations are to be expected
                        }
                    },
                    System.Threading.CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
                } else if (id == (int)HotKey.CursorCapture) {
                    _manager.CaptureCursor();
                }
            }
        }

        public void ReloadHotkeys() {
            UnregisterHotKeys();

            foreach (HotKey hotKey in (HotKey[])Enum.GetValues(typeof(HotKey))) {
                Keys key = Keys.None;
                try
                {
                    switch (hotKey)
                    {
                        case HotKey.StartStop: key = Settings.Default.StartStopHotKey; break;
                        case HotKey.CursorCapture: key = Settings.Default.CursorCaptureHotKey; break;
                        default: continue;
                    }

                    KeyModifier modifiers = RemoveAndReturnModifiers(ref key);
                    Win32.RegisterHotKey(Handle, (int)hotKey, (int)modifiers, (int)key);

                } catch(Exception)
                {
                    Console.WriteLine($@"Unable to load Hotkey: {key}");
                }
            }
        }

        public void UnregisterHotKeys() {
            // Unregister all hotkeys before closing the form.
            foreach (HotKey hotKey in (HotKey[])Enum.GetValues(typeof(HotKey)))
                Win32.UnregisterHotKey(Handle, (int)hotKey);
        }

        private KeyModifier RemoveAndReturnModifiers(ref Keys key) {
            var modifiers = KeyModifier.None;

            modifiers |= RemoveAndReturnModifier(ref key, Keys.Shift, KeyModifier.Shift);
            modifiers |= RemoveAndReturnModifier(ref key, Keys.Control, KeyModifier.Control);
            modifiers |= RemoveAndReturnModifier(ref key, Keys.Alt, KeyModifier.Alt);

            return modifiers;
        }

        private KeyModifier RemoveAndReturnModifier(ref Keys key, Keys keyModifier, KeyModifier modifier)
        {
            if ((key & keyModifier) != 0)
            {
                key &= ~keyModifier;
                return modifier;
            }

            return KeyModifier.None;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKeys();
        }

        private readonly Manager _manager;
        private static int WM_HOTKEY = 0x0312;


        private void btnAbout_Click(object sender, EventArgs e)
        {
            about.GetForm.Show();
        }

        private void ToggleButtonEnabledRunning()
        {
            btnSettings.Enabled = false;
            btnStop.Enabled     = true;
        }

        private void ToggleButtonEnabledNotRunning()
        {
            btnSettings.Enabled = true;
            btnStop.Enabled     = false;
        }

        public void Started()
        {
            ToggleButtonEnabledRunning();
            btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_PAUSE");
            lblStatus.Image = Resources.online;
        }

        public void Stopped()
        {
            ToggleButtonEnabledNotRunning();
            btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_START");
            lblStatus.Image = Resources.offline;
        }

        public void Resumed()
        {
            ToggleButtonEnabledRunning();
            btnStart.Text   = Translate.GetTranslate("frmMain", "BUTTON_PAUSE");
            lblStatus.Image = Resources.online;
        }

        public void Paused()
        {
            btnSettings.Enabled = true;
            btnStart.Text       = Translate.GetTranslate("frmMain", "BUTTON_RESUME");
            lblStatus.Image     = Resources.online;
        }
    }
}
