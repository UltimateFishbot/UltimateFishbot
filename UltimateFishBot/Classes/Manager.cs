using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes.BodyParts;

namespace UltimateFishBot.Classes
{
    public class Manager
    {
        public enum FishingState
        {
            Idle = 0,
            Start = 1,
            CastingAndSearchingForBobber = 3,
            WaitingForFish = 4,
            Looting = 5,
            Paused = 6,
            Stopped = 7
        }

        public enum NeededAction
        {
            None = 0x00,
            HearthStone = 0x01,
            Lure = 0x02,
            Charm = 0x04,
            Raft = 0x08,
            Bait = 0x10,
            AntiAfkMove = 0x20
        }

        private CancellationTokenSource _cancellationTokenSource;
        private System.Windows.Forms.Timer m_LureTimer;
        private System.Windows.Forms.Timer m_HearthStoneTimer;
        private System.Windows.Forms.Timer m_RaftTimer;
        private System.Windows.Forms.Timer m_CharmTimer;
        private System.Windows.Forms.Timer m_BaitTimer;
        private System.Windows.Forms.Timer m_AntiAfkTimer;

        private int m_fishWaitTime;

        private frmMain m_mainForm;

        private Eyes m_eyes;
        private Hands m_hands;
        private Ears m_ears;
        private Mouth m_mouth;
        private Legs m_legs;
        private T2S t2s;

        private NeededAction m_neededActions;
        private FishingState m_fishingState;
        private FishingStats m_fishingStats;

        private const int SECOND = 1000;
        private const int MINUTE = 60 * SECOND;
        private const int ACTION_TIMER_LENGTH = 500;

        public Manager(frmMain mainForm)
        {
            m_mainForm = mainForm;

            m_eyes = new Eyes();
            m_hands = new Hands();
            m_ears = new Ears(this);
            m_mouth = new Mouth(m_mainForm);
            m_legs = new Legs();

            m_fishingState = FishingState.Stopped;
            m_neededActions = NeededAction.None;

            m_fishingStats = new FishingStats();
            m_fishingStats.Reset();

            _cancellationTokenSource = null;

            //InitializeTimer(Timer,                Handler);
            InitializeTimer(ref m_LureTimer, LureTimerTick);
            InitializeTimer(ref m_CharmTimer, CharmTimerTick);
            InitializeTimer(ref m_RaftTimer, RaftTimerTick);
            InitializeTimer(ref m_BaitTimer, BaitTimerTick);
            InitializeTimer(ref m_HearthStoneTimer, HearthStoneTimerTick);
            InitializeTimer(ref m_AntiAfkTimer, AntiAfkTimerTick);

            ResetTimers();
        }

        private void InitializeTimer(ref System.Windows.Forms.Timer timer, EventHandler handler)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = false;
            timer.Tick += new EventHandler(handler);
        }

        public async Task RunBotUntilCanceled()
        {
            ResetTimers();
            EnableTimers();
            await RunBot();
        }

        public async Task Resume()
        {
            await RunBot();
        }

        private async Task RunBot()
        {
            SeFishingState(FishingState.Start);
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.WhenAll(new[]
                {
                    TakeActions(_cancellationTokenSource.Token),
                    m_ears.Listen(_cancellationTokenSource.Token),
                });
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }

        public void Pause()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            SeFishingState(FishingState.Paused);
        }

        public void EnableTimers()
        {
            if (Properties.Settings.Default.AutoLure)
            {
                AddNeededAction(NeededAction.Lure);
                m_LureTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoCharm)
            {
                AddNeededAction(NeededAction.Charm);
                m_CharmTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoRaft)
            {
                AddNeededAction(NeededAction.Raft);
                m_RaftTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoBait)
            {
                AddNeededAction(NeededAction.Bait);
                m_BaitTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoHearth)
                m_HearthStoneTimer.Enabled = true;

            if (Properties.Settings.Default.AntiAfk)
                m_AntiAfkTimer.Enabled = true;
        }

        public void Stop()
        {
            // only cancel if not already stopped/paused
            if (!IsStoppedOrPaused())
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            if (GetCurrentState() != FishingState.Stopped)
            {
                m_LureTimer.Enabled = false;
                m_RaftTimer.Enabled = false;
                m_CharmTimer.Enabled = false;
                m_BaitTimer.Enabled = false;
                m_HearthStoneTimer.Enabled = false;
                SeFishingState(FishingState.Stopped);
            }
        }

        public void SeFishingState(FishingState newState)
        {
            if (IsStoppedOrPaused())
                if (newState != FishingState.Start)
                    return;

            FishingState prevState = m_fishingState;
            m_fishingState = newState;

            if (m_fishingState == FishingState.Idle) // If we start a new loop, check why and increase stats according
            {
                UpdateStats(prevState, m_fishingStats);
            }
        }

        private static void UpdateStats(FishingState prevState, FishingStats stats)
        {
            switch (prevState)
            {
                case FishingState.Looting:
                    {
                        stats.RecordSuccess();
                        break;
                    }
                case FishingState.CastingAndSearchingForBobber:
                    {
                        stats.RecordBobberNotFound();
                        break;
                    }
                case FishingState.WaitingForFish:
                    {
                        stats.RecordNotHeard();
                        break;
                    }
            }
        }

        public FishingState GetCurrentState()
        {
            return m_fishingState;
        }

        public bool IsStoppedOrPaused()
        {
            return GetCurrentState() == FishingState.Stopped || GetCurrentState() == FishingState.Paused;
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

        private void ResetTimers()
        {
            m_LureTimer.Interval = Properties.Settings.Default.LureTime * MINUTE + 22 * SECOND;
            m_RaftTimer.Interval = Properties.Settings.Default.RaftTime * MINUTE;
            m_CharmTimer.Interval = Properties.Settings.Default.CharmTime * MINUTE;
            m_BaitTimer.Interval = Properties.Settings.Default.BaitTime * MINUTE;
            m_HearthStoneTimer.Interval = Properties.Settings.Default.HearthTime * MINUTE;
            m_AntiAfkTimer.Interval = Properties.Settings.Default.AntiAfkTime * MINUTE;

            m_fishWaitTime = 0;
        }

        public async Task HearFish()
        {
            if (GetCurrentState() != FishingState.WaitingForFish)
                return;

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_HEAR_FISH"));

            SeFishingState(FishingState.Looting);
            await m_hands.Loot();
            m_fishWaitTime = 0;
            SeFishingState(FishingState.Idle);
        }

        private async Task TakeActions(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await Task.Delay(ACTION_TIMER_LENGTH, cancellationToken);
                    await TakeNextAction(cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task TakeNextAction(CancellationToken cancellationToken)
        {
            switch (GetCurrentState())
            {
                case FishingState.Start:
                    {
                        // We just start, going to Idle to begin bot loop
                        SeFishingState(FishingState.Idle);
                        break;
                    }
                case FishingState.Idle:
                    {
                        // We first check if another action is needed, foreach on all NeededAction enum values
                        foreach (NeededAction neededAction in (NeededAction[])Enum.GetValues(typeof(NeededAction)))
                        {
                            if (HasNeededAction(neededAction))
                            {
                                await HandleNeededAction(neededAction);
                                return;
                            }
                        }

                        // If no other action required, we can cast !
                        SeFishingState(FishingState.CastingAndSearchingForBobber);
                        break;
                    }
                case FishingState.CastingAndSearchingForBobber:
                    {
                        m_mouth.Say(Translate.GetTranslate("manager", "LABEL_CASTING"));
                        await m_hands.Cast();

                        m_mouth.Say(Translate.GetTranslate("manager", "LABEL_FINDING"));
                        bool didFindFish = await m_eyes.LookForBobber(cancellationToken);
                        if (didFindFish)
                        {
                            SeFishingState(Manager.FishingState.WaitingForFish);
                        }
                        else
                        {
                            // Refactor this later: record failure stat here
                            SeFishingState(Manager.FishingState.Idle);
                        }
                        // We are just waiting for the Eyes
                        break;
                    }
                case FishingState.WaitingForFish:
                    {
                        // We are waiting a detection from the Ears
                        m_mouth.Say(Translate.GetTranslate(
                            "manager",
                            "LABEL_WAITING",
                            GetFishWaitTime() / 1000,
                            Properties.Settings.Default.FishWait / 1000));

                        if ((m_fishWaitTime += ACTION_TIMER_LENGTH) >= Properties.Settings.Default.FishWait)
                        {
                            SeFishingState(FishingState.Idle);
                            m_fishWaitTime = 0;
                        }

                        break;
                    }
            }
        }

        private async Task HandleNeededAction(NeededAction action)
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
                    await m_hands.DoAction(action, m_mouth);
                    break;
                case NeededAction.AntiAfkMove:
                    await m_legs.DoMovement(t2s);
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
