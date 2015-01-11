using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes.BodyParts;
using UltimateFishBot.Properties;

namespace UltimateFishBot.Classes
{
    class Manager
    {
        public enum FishingState
        {
            Idle                = 0,
            Start               = 1,
            Casting             = 2,
            SearchingForBobber  = 3,
            WaitingForFish      = 4,
            Looting             = 5,
            Paused              = 6,
            Stopped             = 7
        }

        public enum NeededAction
        {
            None        = 0x00,
            HearthStone = 0x01,
            Lure        = 0x02,
            Charm       = 0x04,
            Raft        = 0x08,
            Bait        = 0x10
        }

        private Timer m_nextActionTimer;
        private Timer m_LureTimer;
        private Timer m_HearthStoneTimer;
        private Timer m_RaftTimer;
        private Timer m_CharmTimer;
        private Timer m_BaitTimer;

        private int   m_fishWaitTime;

        private frmMain m_mainForm;

        private Eyes  m_eyes;
        private Hands m_hands;
        private Ears  m_ears;
        private Mouth m_mouth;

        private NeededAction m_neededActions;
        private FishingState m_actualState;

        private const int SECOND                = 1000;
        private const int MINUTE                = 60 * SECOND;
        private const int ACTION_TIMER_LENGTH   = 500;

        public Manager(frmMain mainForm)
        {
            m_mainForm      = mainForm;

            m_eyes          = new Eyes(this);
            m_hands         = new Hands();
            m_ears          = new Ears(this);
            m_mouth         = new Mouth(m_mainForm);

            m_actualState   = FishingState.Stopped;
            m_neededActions = NeededAction.None;

            //InitializeTimer(Timer,                Handler);
            InitializeTimer(ref m_nextActionTimer,  TakeNextAction);
            InitializeTimer(ref m_LureTimer,        LureTimerTick);
            InitializeTimer(ref m_CharmTimer,       CharmTimerTick);
            InitializeTimer(ref m_RaftTimer,        RaftTimerTick);
            InitializeTimer(ref m_BaitTimer,        BaitTimerTick);
            InitializeTimer(ref m_HearthStoneTimer, HearthStoneTimerTick);

            ResetTimers();
        }

        private void InitializeTimer(ref Timer timer, EventHandler handler)
        {
            timer = new Timer();
            timer.Enabled = false;
            timer.Tick += new EventHandler(handler);
        }

        public void Start()
        {
            if (GetActualState() == FishingState.Stopped)
                ReStart();
            else if (GetActualState() == FishingState.Paused)
                Resume();
        }

        public void ReStart()
        {
            ResetTimers();
            SwitchTimerState(true);
            
            if (Properties.Settings.Default.AutoLure)
                AddNeededAction(NeededAction.Lure);

            if (Properties.Settings.Default.AutoCharm)
                AddNeededAction(NeededAction.Charm);

            if (Properties.Settings.Default.AutoRaft)
                AddNeededAction(NeededAction.Raft);

            if (Properties.Settings.Default.AutoBait)
                AddNeededAction(NeededAction.Bait);

            SetActualState(FishingState.Start);
        }

        public void Resume()
        {
            SetActualState(FishingState.Start);
        }

        public void Stop()
        {
            SwitchTimerState(false);
            SetActualState(FishingState.Stopped);
        }

        public void Pause()
        {
            SetActualState(FishingState.Paused);
        }

        public void SetActualState(FishingState state)
        {
            if (m_actualState == FishingState.Stopped || m_actualState == FishingState.Paused)
                if (state != FishingState.Start)
                    return;

            m_actualState = state;
        }

        public FishingState GetActualState()
        {
            return m_actualState;
        }

        public int GetFishWaitTime()
        {
            return m_fishWaitTime;
        }

        private void SwitchTimerState(bool enabled)
        {
            // For activation, we check that the corresponding settings are sets
            if (enabled)
            {
                m_nextActionTimer.Enabled = true;

                if (Properties.Settings.Default.AutoLure)
                    m_LureTimer.Enabled = true;

                if (Properties.Settings.Default.AutoRaft)
                    m_RaftTimer.Enabled = true;

                if (Properties.Settings.Default.AutoCharm)
                    m_CharmTimer.Enabled = true;

                if (Properties.Settings.Default.AutoBait)
                    m_BaitTimer.Enabled = true;

                if (Properties.Settings.Default.AutoHearth)
                    m_HearthStoneTimer.Enabled = true;
            }
            // On deactivation, we don't care
            else
            {
                m_nextActionTimer.Enabled   = false;
                m_LureTimer.Enabled         = false;
                m_RaftTimer.Enabled         = false;
                m_CharmTimer.Enabled        = false;
                m_BaitTimer.Enabled         = false;
                m_HearthStoneTimer.Enabled  = false;
            }
        }

        private void ResetTimers()
        {
            m_nextActionTimer.Interval  = ACTION_TIMER_LENGTH;
            m_LureTimer.Interval        = Properties.Settings.Default.LureTime   * MINUTE;
            m_RaftTimer.Interval        = Properties.Settings.Default.RaftTime   * MINUTE;
            m_CharmTimer.Interval       = Properties.Settings.Default.CharmTime  * MINUTE;
            m_BaitTimer.Interval        = Properties.Settings.Default.BaitTime   * MINUTE;
            m_HearthStoneTimer.Interval = Properties.Settings.Default.HearthTime * MINUTE;

            m_fishWaitTime              = 0;
        }

        public void HearFish()
        {
            if (GetActualState() != FishingState.WaitingForFish)
                return;

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_HEAR_FISH"));

            SetActualState(FishingState.Looting);
            m_hands.Loot();
            SetActualState(FishingState.Idle);
        }

        private void TakeNextAction(Object myObject, EventArgs myEventArgs)
        {
            switch (GetActualState())
            {
                case FishingState.Start:
                {
                    // We just start, going to Idle to begin bot loop
                    SetActualState(FishingState.Idle);
                    break;
                }
                case FishingState.Idle:
                {
                    // We first check if another action is needed, foreach on all NeededAction enum values
                    foreach (NeededAction neededAction in (NeededAction[])Enum.GetValues(typeof(NeededAction)))
                    {
                        if (HasNeededAction(neededAction))
                        {
                            m_hands.DoAction(neededAction, m_mouth);
                            RemoveNeededAction(neededAction);

                            if (neededAction == NeededAction.HearthStone)
                                m_mainForm.StopFishing();

                            return;
                        }
                    }

                    // If no other action required, we can cast !
                    m_mouth.Say(Translate.GetTranslate("manager", "LABEL_CASTING"));
                    SetActualState(FishingState.Casting);
                    m_hands.Cast();
                    break;
                }
                case FishingState.Casting:
                {
                    m_mouth.Say(Translate.GetTranslate("manager", "LABEL_START_FINDING"));
                    SetActualState(FishingState.SearchingForBobber);
                    m_eyes.StartLooking(); // <= The new state will be set in the Eyes
                    break;
                }
                case FishingState.SearchingForBobber:
                {
                    // We are just waiting for the Eyes
                    m_mouth.Say(Translate.GetTranslate("manager", "LABEL_FINDING"));
                    break;
                }
                case FishingState.WaitingForFish:
                {
                    // We are waiting a detection from the Ears
                    m_mouth.Say(Translate.GetTranslate("manager", "LABEL_WAITING", GetFishWaitTime() / 1000, Properties.Settings.Default.FishWait / 1000));

                    if ((m_fishWaitTime += ACTION_TIMER_LENGTH) >= Properties.Settings.Default.FishWait)
                    {
                        m_actualState = FishingState.Idle;
                        m_fishWaitTime = 0;
                    }

                    break;
                }
            }
        }

        private void LureTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.Lure);
        }

        private void RaftTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.Raft);
        }

        private void CharmTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.Charm);
        }

        private void BaitTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.Bait);
        }

        private void HearthStoneTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.HearthStone);
        }

        private void AddNeededAction(NeededAction action)
        {
            m_neededActions |= action;
        }

        private void RemoveNeededAction(NeededAction action)
        {
            m_neededActions &= ~action;
        }

        private bool HasNeededAction(NeededAction action)
        {
            return (m_neededActions & action) != NeededAction.None;
        }
    }
}
