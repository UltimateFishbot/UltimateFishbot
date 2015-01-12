using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes;

namespace UltimateFishBot.Forms
{
    public partial class frmSettings : Form
    {
        private enum TabulationIndex
        {
            GeneralFishing  = 0,
            FindCursor      = 1,
            HearingFishing  = 2,
            Premium         = 3,
            Language        = 4
        };

        private MMDevice SndDevice;

        public frmSettings()
        {
            InitializeComponent();
            SndDevice = null;

            tmeAudio.Tick += new EventHandler(tmeAudio_Tick);
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            /*
             * Set Text from translate file
             */

            this.Text                       = Translate.GetTranslate("frmSettings", "TITLE");

            tabSettings.TabPages[(int)TabulationIndex.GeneralFishing].Text  = Translate.GetTranslate("frmSettings", "TAB_TITLE_GENERAL_FISHING");
            tabSettings.TabPages[(int)TabulationIndex.FindCursor].Text      = Translate.GetTranslate("frmSettings", "TAB_TITLE_FIND_CURSOR");
            tabSettings.TabPages[(int)TabulationIndex.HearingFishing].Text  = Translate.GetTranslate("frmSettings", "TAB_TITLE_HEARING_FISH");
            tabSettings.TabPages[(int)TabulationIndex.Premium].Text         = Translate.GetTranslate("frmSettings", "TAB_TITLE_PREMIUM");
            tabSettings.TabPages[(int)TabulationIndex.Language].Text        = Translate.GetTranslate("frmSettings", "TAB_TITLE_LANGUAGE");

            /// General

            LabelDelayCast.Text             = Translate.GetTranslate("frmSettings", "LABEL_DELAY_AFTER_CAST");
            LabelDelayCastDesc.Text         = Translate.GetTranslate("frmSettings", "LABEL_DELAY_AFTER_CAST_DESC");

            LabelFishWait.Text              = Translate.GetTranslate("frmSettings", "LABEL_FISH_WAIT_LIMIT");
            LabelFishWaitDesc.Text          = Translate.GetTranslate("frmSettings", "LABEL_FISH_WAIT_LIMIT_DESC");

            LabelDelayLooting.Text          = Translate.GetTranslate("frmSettings", "LABEL_DELAY_AFTER_LOOTING");
            LabelDelayLootingDesc.Text      = Translate.GetTranslate("frmSettings", "LABEL_DELAY_AFTER_LOOTING_DESC");

            /// Finding the Cursor

            LabelScanningSteps.Text         = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_STEPS");
            LabelScanningStepsDesc.Text     = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_STEPS_DESC");

            LabelScanningDelay.Text         = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_DELAY");
            LabelScanningDelayDesc.Text     = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_DELAY_DESC");

            LabelScanningRetries.Text       = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_RETRIES");
            LabelScanningRetriesDesc.Text   = Translate.GetTranslate("frmSettings", "LABEL_SCANNING_RETRIES_DESC");

            cmbCompareIcon.Text             = Translate.GetTranslate("frmSettings", "LABEL_CHECK_ICON");
            LabelCheckCursorIcon.Text       = Translate.GetTranslate("frmSettings", "LABEL_CHECK_ICON_DESC");

            /// Hearing the Fish

            LabelSplashThreshold.Text       = Translate.GetTranslate("frmSettings", "LABEL_SPLASH_THRESHOLD");
            LabelSplashThresholdDesc.Text   = Translate.GetTranslate("frmSettings", "LABEL_SPLASH_THRESHOLD_DESC");

            LabelAudioDevice.Text           = Translate.GetTranslate("frmSettings", "LABEL_AUDIO_DEVICE");
            LabelAudioDeviceDesc.Text       = Translate.GetTranslate("frmSettings", "LABEL_AUDIO_DEVICE_DESC");

            /// Premium Settings

            LabelCastKey.Text               = Translate.GetTranslate("frmSettings", "LABEL_CAST_KEY");
            LabelLureKey.Text               = Translate.GetTranslate("frmSettings", "LABEL_LURE_KEY");
            LabelHearthKey.Text             = Translate.GetTranslate("frmSettings", "LABEL_HEARTHSTONE_KEY");
            LabelRaftKey.Text               = Translate.GetTranslate("frmSettings", "LABEL_RAFT_KEY");
            LabelCharmKey.Text              = Translate.GetTranslate("frmSettings", "LABEL_CHARM_KEY");
            LabelBaitKey1.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_1");
            LabelBaitKey2.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_2");
            LabelBaitKey3.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_3");
            LabelBaitKey4.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_4");
            LabelBaitKey5.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_5");
            LabelBaitKey6.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_6");
            LabelBaitKey7.Text              = Translate.GetTranslate("frmSettings", "LABEL_BAIT_KEY_7");

            LabelCustomizeDesc.Text         = Translate.GetTranslate("frmSettings", "LABEL_CUSTOMIZE_DESC");

            cbAlt.Text                      = Translate.GetTranslate("frmSettings", "CB_ALT_KEY");
            cbAutoLure.Text                 = Translate.GetTranslate("frmSettings", "CB_AUTO_LURE");
            cbHearth.Text                   = Translate.GetTranslate("frmSettings", "CB_AUTO_HEARTHSTONE");
            cbApplyRaft.Text                = Translate.GetTranslate("frmSettings", "CB_AUTO_RAFT");
            cbApplyCharm.Text               = Translate.GetTranslate("frmSettings", "CB_AUTO_CHARM");
            cbAutoBait.Text                 = Translate.GetTranslate("frmSettings", "CB_AUTO_BAIT");
            cbRandomBait.Text               = Translate.GetTranslate("frmSettings", "CB_RANDOM_BAIT");
            cbShiftLoot.Text                = Translate.GetTranslate("frmSettings", "CB_SHIFT_LOOT");

            LabelProcessName.Text           = Translate.GetTranslate("frmSettings", "LABEL_PROCESS_NAME");
            LabelProcessNameDesc.Text       = Translate.GetTranslate("frmSettings", "LABEL_PROCESS_NAME_DESC");

            /// Language Settings

            labelLanguage.Text              = Translate.GetTranslate("frmSettings", "LABEL_LANGUAGE");
            labelLanguageDesc.Text          = Translate.GetTranslate("frmSettings", "LABEL_LANGUAGE_DESC");

            /// Buttons

            buttonSave.Text                 = Translate.GetTranslate("frmSettings", "BUTTON_SAVE");
            buttonCancel.Text               = Translate.GetTranslate("frmSettings", "BUTTON_CANCEL");

            /*
             * Set Settings from save
             */

            /// General
            txtCastDelay.Text       = Properties.Settings.Default.CastingDelay.ToString();
            txtLootingDelay.Text    = Properties.Settings.Default.LootingDelay.ToString();
            txtFishWait.Text        = Properties.Settings.Default.FishWait.ToString();

            /// Finding the Cursor
            txtDelay.Text           = Properties.Settings.Default.ScanningDelay.ToString();
            txtRetries.Text         = Properties.Settings.Default.ScanningRetries.ToString();
            txtScanSteps.Text       = Properties.Settings.Default.ScanningSteps.ToString();
            cmbCompareIcon.Checked  = Properties.Settings.Default.CheckCursor;

            /// Hearing the Fish
            txtSplash.Text          = Properties.Settings.Default.SplashLimit.ToString();
            LoadAudioDevices();

            /// Premium Settings
            txtProcName.Text        = Properties.Settings.Default.ProcName;
            cbAutoLure.Checked      = Properties.Settings.Default.AutoLure;
            cbHearth.Checked        = Properties.Settings.Default.SwapGear;
            cbAlt.Checked           = Properties.Settings.Default.UseAltKey;

            txtFishKey.Text         = Properties.Settings.Default.FishKey;
            txtLureKey.Text         = Properties.Settings.Default.LureKey;
            txtHearthKey.Text       = Properties.Settings.Default.HearthKey;
            cbHearth.Checked        = Properties.Settings.Default.AutoHearth;
            txtHearthTime.Text      = Properties.Settings.Default.HearthTime.ToString();

            // MoP Premium (Angler's Raft & Ancient Pandaren Fishing Charm)
            txtCharmKey.Text        = Properties.Settings.Default.CharmKey;
            txtRaftKey.Text         = Properties.Settings.Default.RaftKey;
            cbApplyRaft.Checked     = Properties.Settings.Default.AutoRaft;
            cbApplyCharm.Checked    = Properties.Settings.Default.AutoCharm;
            cbShiftLoot.Checked     = Properties.Settings.Default.ShiftLoot;

            // WoD Premium (Bait)
            txtBaitKey1.Text        = Properties.Settings.Default.BaitKey1;
            txtBaitKey2.Text        = Properties.Settings.Default.BaitKey2;
            txtBaitKey3.Text        = Properties.Settings.Default.BaitKey3;
            txtBaitKey4.Text        = Properties.Settings.Default.BaitKey4;
            txtBaitKey5.Text        = Properties.Settings.Default.BaitKey5;
            txtBaitKey6.Text        = Properties.Settings.Default.BaitKey6;
            txtBaitKey7.Text        = Properties.Settings.Default.BaitKey7;
            cbAutoBait.Checked      = Properties.Settings.Default.AutoBait;
            cbRandomBait.Checked    = Properties.Settings.Default.randomBait;

            //Times
            txtLureTime.Text        = Properties.Settings.Default.LureTime.ToString();
            txtHearthTime.Text      = Properties.Settings.Default.HearthTime.ToString();
            txtRaftTime.Text        = Properties.Settings.Default.RaftTime.ToString();
            txtCharmTime.Text       = Properties.Settings.Default.CharmTime.ToString();
            txtBaitTime.Text        = Properties.Settings.Default.BaitTime.ToString();

            LoadLanguages();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            /// General
            Properties.Settings.Default.CastingDelay    = int.Parse(txtCastDelay.Text);
            Properties.Settings.Default.LootingDelay    = int.Parse(txtLootingDelay.Text);
            Properties.Settings.Default.FishWait        = int.Parse(txtFishWait.Text);
            
            /// Finding the Cursor
            Properties.Settings.Default.ScanningDelay   = int.Parse(txtDelay.Text);
            Properties.Settings.Default.ScanningRetries = int.Parse(txtRetries.Text);
            Properties.Settings.Default.ScanningSteps   = int.Parse(txtScanSteps.Text);
            Properties.Settings.Default.CheckCursor     = cmbCompareIcon.Checked;
            
            /// Hearing the Fish
            Properties.Settings.Default.SplashLimit     = int.Parse(txtSplash.Text);
            Properties.Settings.Default.AudioDevice     = (string)cmbAudio.SelectedValue;

            /// Premium Settings

            Properties.Settings.Default.ProcName        = txtProcName.Text;
            Properties.Settings.Default.AutoLure        = cbAutoLure.Checked;
            Properties.Settings.Default.SwapGear        = cbHearth.Checked;
            Properties.Settings.Default.UseAltKey       = cbAlt.Checked;

            Properties.Settings.Default.FishKey         = txtFishKey.Text;
            Properties.Settings.Default.LureKey         = txtLureKey.Text;
            Properties.Settings.Default.HearthKey       = txtHearthKey.Text;
            Properties.Settings.Default.AutoHearth      = cbHearth.Checked;

            // MoP Premium (Angler's Raft & Ancient Pandaren Fishing Charm)
            Properties.Settings.Default.CharmKey        = txtCharmKey.Text;
            Properties.Settings.Default.RaftKey         = txtRaftKey.Text;
            Properties.Settings.Default.AutoRaft        = cbApplyRaft.Checked;
            Properties.Settings.Default.AutoCharm       = cbApplyCharm.Checked;
            Properties.Settings.Default.ShiftLoot       = cbShiftLoot.Checked;

            // WoD Premium (Bait)
            Properties.Settings.Default.BaitKey1        = txtBaitKey1.Text;
            Properties.Settings.Default.BaitKey2        = txtBaitKey2.Text;
            Properties.Settings.Default.BaitKey3        = txtBaitKey3.Text;
            Properties.Settings.Default.BaitKey4        = txtBaitKey4.Text;
            Properties.Settings.Default.BaitKey5        = txtBaitKey5.Text;
            Properties.Settings.Default.BaitKey6        = txtBaitKey6.Text;
            Properties.Settings.Default.BaitKey7        = txtBaitKey7.Text;
            Properties.Settings.Default.AutoBait        = cbAutoBait.Checked;
            Properties.Settings.Default.randomBait      = cbRandomBait.Checked;

            //Times
            Properties.Settings.Default.LureTime        = int.Parse(txtLureTime.Text);
            Properties.Settings.Default.HearthTime      = int.Parse(txtHearthTime.Text);
            Properties.Settings.Default.RaftTime        = int.Parse(txtRaftTime.Text);
            Properties.Settings.Default.CharmTime       = int.Parse(txtCharmTime.Text);
            Properties.Settings.Default.BaitTime        = int.Parse(txtBaitTime.Text);

            if ((string)cmbLanguage.SelectedItem != Properties.Settings.Default.Language)
            {
                Properties.Settings.Default.Language = (string)cmbLanguage.SelectedItem;
                Properties.Settings.Default.Save();

                MessageBox.Show(Translate.GetTranslate("frmSettings", "LABEL_LANGUAGE_CHANGE"),
                                Translate.GetTranslate("frmSettings", "TITLE_LANGUAGE_CHANGE"));

                Thread.Sleep(1000);
                Application.Restart();
            }
            else
            {
                Properties.Settings.Default.Save();
                this.Close();
            }
        }

        private void tabSettings_SelectedIndexChanged(Object sender, EventArgs e)
        {
            tmeAudio.Enabled = (tabSettings.SelectedIndex == 2);
        }

        private void LoadAudioDevices()
        {
            List<Tuple<string, string>> audioDevices = new List<Tuple<string, string>>();
            audioDevices.Add(new Tuple<string, string>("Default", ""));

            try
            {
                MMDeviceEnumerator sndDevEnum = new MMDeviceEnumerator();
                MMDeviceCollection audioCollection = sndDevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATEMASK_ALL);

                // Try to add each audio endpoint to our collection
                for (int i = 0; i < audioCollection.Count; ++i)
                {
                    MMDevice device = audioCollection[i];
                    audioDevices.Add(new Tuple<string, string>(device.FriendlyName, device.ID));
                }
            }
            catch (Exception)
            { }

            // Setup the display
            cmbAudio.Items.Clear();
            cmbAudio.DisplayMember = "Item1";
            cmbAudio.ValueMember = "Item2";
            cmbAudio.DataSource = audioDevices;
            cmbAudio.SelectedValue = Properties.Settings.Default.AudioDevice;
        }

        private void LoadLanguages()
        {
            string[] languageFiles = Directory.GetFiles("./Resources/", "*.xml");
            cmbLanguage.Items.Clear();

            foreach (string file in languageFiles)
            {
                string tmpFile = file.Substring(12); // Remove the "./Resources/" part
                tmpFile = tmpFile.Substring(0, tmpFile.Length - 4); // Remove the  ".xml" part
                cmbLanguage.Items.Add(tmpFile);
            }

            cmbLanguage.SelectedItem = Properties.Settings.Default.Language;
        }
        
        private void tmeAudio_Tick(Object sender, EventArgs e)
        {
            if (SndDevice != null)
            {
                try
                {
                    int currentVolumnLevel = (int)(SndDevice.AudioMeterInformation.MasterPeakValue * 100);
                    pgbSoundLevel.Value = currentVolumnLevel;
                    lblAudioLevel.Text = currentVolumnLevel.ToString();
                }
                catch (Exception)
                {
                    pgbSoundLevel.Value = 0;
                    lblAudioLevel.Text = "0";
                }
            }
            else
            {
                pgbSoundLevel.Value = 0;
                lblAudioLevel.Text = "0";
            }
        }

        private void cmbAudio_SelectedIndexChanged(object sender, EventArgs e)
        {
            MMDeviceEnumerator sndDevEnum = new MMDeviceEnumerator();

            if (!string.IsNullOrEmpty((string)cmbAudio.SelectedValue))
                SndDevice = sndDevEnum.GetDevice((string)cmbAudio.SelectedValue);
            else
                SndDevice = sndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }
    }
}
