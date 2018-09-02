using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using UltimateFishBot.Helpers;

namespace UltimateFishBot.BodyParts
{
    internal class Hands
    {
        private Cursor _mCursor;
        private int _mBaitIndex;
        private string[] _mBaitKeys;
        private IntPtr _wow;

        private int _aCastingDelay;
        private int _aLootingDelay;

        public Hands(IntPtr wowWindow)
        {
            _wow = wowWindow;
            _mBaitIndex = 0;
            if (Cursor.Current != null) _mCursor = new Cursor(Cursor.Current.Handle);
            UpdateKeys();
        }

        public void SetWow(IntPtr wowWindow) {
            _wow = wowWindow;
        }

        private void UpdateKeys()
        {
            _mBaitKeys = new[]
            {
                Properties.Settings.Default.BaitKey1,
                Properties.Settings.Default.BaitKey2,
                Properties.Settings.Default.BaitKey3,
                Properties.Settings.Default.BaitKey4,
                Properties.Settings.Default.BaitKey5,
                Properties.Settings.Default.BaitKey6,
                Properties.Settings.Default.BaitKey7,
            };
        }

        public async Task Cast(CancellationToken token)
        {
            if (Properties.Settings.Default.RightClickCast)  {
                Win32.SendMouseDblRightClick(_wow);
            } else {
                Win32.SendKey(Properties.Settings.Default.FishKey);
                Log.Information("Sent key: " + Properties.Settings.Default.FishKey);
            }
            var rnd = new Random();
            _aCastingDelay = rnd.Next(Properties.Settings.Default.CastingDelayLow, Properties.Settings.Default.CastingDelayHigh);
            await Task.Delay(_aCastingDelay, token);
        }

        public async Task Loot()
        {
            Win32.SendMouseClick(_wow);
            Log.Information("Send Loot.");
            Random rnd = new Random();
            _aLootingDelay = rnd.Next(Properties.Settings.Default.LootingDelayLow, Properties.Settings.Default.LootingDelayHigh);
            await Task.Delay(_aLootingDelay);
        }

        public void ResetBaitIndex()
        {
            _mBaitIndex = 0;
        }

        public async Task DoAction(NeededAction action, Mouth mouth, CancellationToken cancellationToken)
        {
            string actionKey = "";
            int sleepTime = 0;

            switch (action)
            {
                case NeededAction.HearthStone:
                    {
                        actionKey = Properties.Settings.Default.HearthKey;
                        mouth.Say(Translate.GetTranslate("manager", "LABEL_HEARTHSTONE"));
                        sleepTime = 0;
                        break;
                    }
                case NeededAction.Lure:
                    {
                        actionKey = Properties.Settings.Default.LureKey;
                        mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_LURE"));
                        sleepTime = 3;
                        break;
                    }
                case NeededAction.Charm:
                    {
                        actionKey = Properties.Settings.Default.CharmKey;
                        mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_CHARM"));
                        sleepTime = 3;
                        break;
                    }
                case NeededAction.Raft:
                    {
                        actionKey = Properties.Settings.Default.RaftKey;
                        mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_RAFT"));
                        sleepTime = 2;
                        break;
                    }
                case NeededAction.Bait:
                    {
                        int baitIndex = 0;

                        if (Properties.Settings.Default.CycleThroughBaitList)
                        {
                            if (_mBaitIndex >= 6)
                                _mBaitIndex = 0;

                            baitIndex = _mBaitIndex++;
                        }

                        actionKey = _mBaitKeys[baitIndex];
                        mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_BAIT", baitIndex));
                        sleepTime = 3;
                        break;
                    }
                default:
                    return;
            }

            Log.Information("Send key start: " + actionKey);
            Win32.ActivateWow(_wow);
            await Task.Delay(1000, cancellationToken);
            Win32.SendKey(actionKey);
            Log.Information("Sent key: "+actionKey);
            await Task.Delay(sleepTime * 1000, cancellationToken);
        }
    }
}
