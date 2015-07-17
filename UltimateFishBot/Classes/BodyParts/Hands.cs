using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UltimateFishBot.Classes.Helpers;

namespace UltimateFishBot.Classes.BodyParts
{
    class Hands
    {
        private Cursor cursor;
        private string[] baitKeys;

        public Hands()
        {
            cursor = new Cursor(Cursor.Current.Handle);
            UpdateKeys();
        }

        public void UpdateKeys()
        {
            baitKeys = new string[7]
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

        public void Cast()
        {
            Win32.ActivateWow();
            System.Threading.Thread.Sleep(Properties.Settings.Default.CastingDelay);
            Win32.SendKey(Properties.Settings.Default.FishKey);
        }

        public void Loot()
        {
            Win32.SendMouseClick();
            Thread.Sleep(Properties.Settings.Default.LootingDelay);
        }

        public void DoAction(Manager.NeededAction action, Mouth mouth)
        {
            string actionKey = "";
            int sleepTime = 0;

            switch (action)
            {
                case Manager.NeededAction.HearthStone:
                {
                    mouth.Say(Translate.GetTranslate("manager", "LABEL_HEARTHSTONE"));
                    actionKey = Properties.Settings.Default.HearthKey;
                    sleepTime = 0;
                    break;
                }
                case Manager.NeededAction.Lure:
                {
                    mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_LURE"));
                    actionKey = Properties.Settings.Default.LureKey;
                    sleepTime = 3;
                    break;
                }
                case Manager.NeededAction.Charm:
                {
                    mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_CHARM"));
                    actionKey = Properties.Settings.Default.CharmKey;
                    sleepTime = 3;
                    break;
                }
                case Manager.NeededAction.Raft:
                {
                    mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_RAFT"));
                    actionKey = Properties.Settings.Default.RaftKey;
                    sleepTime = 2;
                    break;
                }
                case Manager.NeededAction.Bait:
                {
                    int baitIndex = 0;

                    if (Properties.Settings.Default.randomBait)
                        baitIndex = new Random().Next(0, 7);

                    mouth.Say(Translate.GetTranslate("manager", "LABEL_APPLY_BAIT", baitIndex));
                    actionKey = baitKeys[baitIndex];
                    sleepTime = 3;
                    break;
                }
                default:
                    return;
            }

            Win32.ActivateWow();
            Win32.SendKey(actionKey);
            Thread.Sleep(sleepTime * 1000);
        }
    }
}
