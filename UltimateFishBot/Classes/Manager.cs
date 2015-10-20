using System;
using System.Windows.Forms;
using UltimateFishBot.Classes.BodyParts;

namespace UltimateFishBot.Classes
{
    public class Manager
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
            Bait        = 0x10,
            AntiAfkMove = 0x20
        }

        public struct FishingStats
        {
            public int totalSuccessFishing  { get; set; }
            public int totalNotFoundFish    { get; set; }
            public int totalNotEaredFish    { get; set; }

            public void Reset()
            {
                totalSuccessFishing = 0;
                totalNotFoundFish   = 0;
                totalNotEaredFish   = 0;
            }

            public int Total()
            {
                return totalSuccessFishing + totalNotFoundFish + totalNotEaredFish;
            }
        }

        private Timer m_nextActionTimer;
        private Timer m_LureTimer;
        private Timer m_HearthStoneTimer;
        private Timer m_RaftTimer;
        private Timer m_CharmTimer;
        private Timer m_BaitTimer;
        private Timer m_AntiAfkTimer;

        private int   m_fishWaitTime;

        private frmMain m_mainForm;

        private Eyes  m_eyes;
        private Hands m_hands;
        private Ears  m_ears;
        private Mouth m_mouth;
        private Legs  m_legs;
        private T2S t2s;

        private NeededAction m_neededActions;
        private FishingState m_actualState;
        private FishingStats m_fishingStats;

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
            m_legs          = new Legs();

            m_actualState   = FishingState.Stopped;
            m_neededActions = NeededAction.None;

            m_fishingStats.Reset();

            //InitializeTimer(Timer,                Handler);
            InitializeTimer(ref m_nextActionTimer,  TakeNextAction);
            InitializeTimer(ref m_LureTimer,        LureTimerTick);
            InitializeTimer(ref m_CharmTimer,       CharmTimerTick);
            InitializeTimer(ref m_RaftTimer,        RaftTimerTick);
            InitializeTimer(ref m_BaitTimer,        BaitTimerTick);
            InitializeTimer(ref m_HearthStoneTimer, HearthStoneTimerTick);
            InitializeTimer(ref m_AntiAfkTimer,     AntiAfkTimerTick);

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

        public void SetActualState(FishingState newState)
        {
            if (IsStoppedOrPaused())
                if (newState != FishingState.Start)
                    return;

            UpdateStats(newState);

            m_actualState = newState;
        }

        private void UpdateStats(FishingState newState)
        {
            if (newState == FishingState.Idle) // If we start a new loop, check why and increase stats according
            {
                switch (m_actualState)
                {
                    case FishingState.Looting:
                    {
                        ++m_fishingStats.totalSuccessFishing;
                        break;
                    }
                    case FishingState.Casting:
                    case FishingState.SearchingForBobber:
                    {
                        ++m_fishingStats.totalNotFoundFish;
                        break;
                    }
                    case FishingState.WaitingForFish:
                    {
                        ++m_fishingStats.totalNotEaredFish;
                        break;
                    }
                }
            }
        }

        public FishingState GetActualState()
        {
            return m_actualState;
        }

        public bool IsStoppedOrPaused()
        {
            return GetActualState() == FishingState.Stopped || GetActualState() == FishingState.Paused;
        }

        public FishingStats GetFishingStats()
        {
            return m_fishingStats;
        }

        public void ResetFishingStats()
        {
            m_fishingStats.Reset();
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

                if (Properties.Settings.Default.AntiAfk)
                    m_AntiAfkTimer.Enabled = true;
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
            m_LureTimer.Interval        = Properties.Settings.Default.LureTime * MINUTE + 22 * SECOND;
            m_RaftTimer.Interval        = Properties.Settings.Default.RaftTime      * MINUTE;
            m_CharmTimer.Interval       = Properties.Settings.Default.CharmTime     * MINUTE;
            m_BaitTimer.Interval        = Properties.Settings.Default.BaitTime      * MINUTE;
            m_HearthStoneTimer.Interval = Properties.Settings.Default.HearthTime    * MINUTE;
            m_AntiAfkTimer.Interval     = Properties.Settings.Default.AntiAfkTime   * MINUTE;

            m_fishWaitTime              = 0;
        }

        public void HearFish()
        {
            if (GetActualState() != FishingState.WaitingForFish)
                return;

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_HEAR_FISH"));

            SetActualState(FishingState.Looting);
            m_hands.Loot();
            m_fishWaitTime = 0;
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
                            HandleNeededAction(neededAction);
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
                        SetActualState(FishingState.Idle);
                        m_fishWaitTime = 0;
                    }

                    break;
                }
            }
        }

        private void HandleNeededAction(NeededAction action)
        {
            switch (action)
            {
                case NeededAction.HearthStone:
                    m_mainForm.StopFishing();
                    goto case NeededAction.Lure; // We continue, Hearthstone need m_hands.DoAction
                case NeededAction.Lure:
                case NeededAction.Charm:
                case NeededAction.Raft:
                case NeededAction.Bait:
                    m_hands.DoAction(action, m_mouth);
                    break;
                case NeededAction.AntiAfkMove:
                    m_legs.DoMovement(t2s);
                    break;
            }

            RemoveNeededAction(action);
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

        private void AntiAfkTimerTick(Object myObject, EventArgs myEventArgs)
        {
            AddNeededAction(NeededAction.AntiAfkMove);
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
