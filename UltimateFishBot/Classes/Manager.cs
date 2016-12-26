using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes.BodyParts;

namespace UltimateFishBot.Classes
{
    public interface IManagerEventHandler
    {
        void Started();
        void Stopped();
        void Resumed();
        void Paused();
        void UpdateStatus(string text);
    }

    public class Manager
    {
        private enum FishingState
        {
            Fishing = 3,
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

        private IManagerEventHandler m_managerEventHandler;

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

        public Manager(IManagerEventHandler managerEventHandler)
        {
            m_managerEventHandler = managerEventHandler;

            m_eyes = new Eyes();
            m_hands = new Hands();
            m_ears = new Ears();
            m_mouth = new Mouth(m_managerEventHandler);
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

        public async Task StartOrResumeOrPause()
        {
            if (m_fishingState == Manager.FishingState.Stopped)
            {
                await RunBotUntilCanceled();
            }
            else if (m_fishingState == Manager.FishingState.Paused)
            {
                await Resume();
            }
            else
            {
                Pause();
            }
        }

        private async Task RunBotUntilCanceled()
        {
            ResetTimers();
            EnableTimers();
            m_managerEventHandler.Started();
            await RunBot();
        }

        private async Task Resume()
        {
            m_managerEventHandler.Resumed();
            await RunBot();
        }

        private async Task RunBot()
        {
            SeFishingState(FishingState.Fishing);
            _cancellationTokenSource = new CancellationTokenSource();
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // We first check if another action is needed, foreach on all NeededAction enum values
                    foreach (NeededAction neededAction in (NeededAction[])Enum.GetValues(typeof(NeededAction)))
                    {
                        if (HasNeededAction(neededAction))
                        {
                            await HandleNeededAction(neededAction);
                        }
                    }

                    // If no other action required, we can cast !
                    await Fish(_cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private void Pause()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            SeFishingState(FishingState.Paused);
            m_managerEventHandler.Paused();
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
            m_managerEventHandler.Stopped();
            // only cancel if not already stopped/paused
            if (!IsStoppedOrPaused())
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            if (m_fishingState != FishingState.Stopped)
            {
                m_LureTimer.Enabled = false;
                m_RaftTimer.Enabled = false;
                m_CharmTimer.Enabled = false;
                m_BaitTimer.Enabled = false;
                m_HearthStoneTimer.Enabled = false;
                SeFishingState(FishingState.Stopped);
            }
        }

        private void SeFishingState(FishingState newState)
        {
            if (IsStoppedOrPaused())
                if (newState != FishingState.Fishing)
                    return;

            m_fishingState = newState;
        }

        private bool IsStoppedOrPaused()
        {
            return m_fishingState == FishingState.Stopped || m_fishingState == FishingState.Paused;
        }

        public FishingStats GetFishingStats()
        {
            return m_fishingStats;
        }

        public void ResetFishingStats()
        {
            m_fishingStats.Reset();
        }
        
        public async Task StartOrStop()
        {
            if (IsStoppedOrPaused())
                await StartOrResumeOrPause();
            else
                Stop();
        }

        private void ResetTimers()
        {
            m_LureTimer.Interval = Properties.Settings.Default.LureTime * MINUTE + 22 * SECOND;
            m_RaftTimer.Interval = Properties.Settings.Default.RaftTime * MINUTE;
            m_CharmTimer.Interval = Properties.Settings.Default.CharmTime * MINUTE;
            m_BaitTimer.Interval = Properties.Settings.Default.BaitTime * MINUTE;
            m_HearthStoneTimer.Interval = Properties.Settings.Default.HearthTime * MINUTE;
            m_AntiAfkTimer.Interval = Properties.Settings.Default.AntiAfkTime * MINUTE;
        }

        private async Task Fish(CancellationToken cancellationToken)
        {
            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_CASTING"));
            await m_hands.Cast();

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_FINDING"));
            bool didFindFish = await m_eyes.LookForBobber(cancellationToken);
            if (!didFindFish)
            {
                m_fishingStats.RecordBobberNotFound();
                return;
            }

            // Update UI with wait status            
            var progress = new Progress<long>(msecs =>
            {
                m_mouth.Say(Translate.GetTranslate(
                    "manager",
                    "LABEL_WAITING",
                    msecs / SECOND,
                    Properties.Settings.Default.FishWait / SECOND));
            });
            var uiUpdateCancelTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var uiUpdateCancelToken = uiUpdateCancelTokenSource.Token;
            (new Task(
                async () => await UpdateUIWhileWaitingToHearFish(progress, uiUpdateCancelToken),
                uiUpdateCancelToken,
                TaskCreationOptions.LongRunning)
                ).Start();

            bool fishHeard = await m_ears.Listen(
                Properties.Settings.Default.FishWait,
                cancellationToken);

            uiUpdateCancelTokenSource.Cancel();

            if (!fishHeard)
            {
                m_fishingStats.RecordNotHeard();
                return;
            }

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_HEAR_FISH"));
            await m_hands.Loot();
            m_fishingStats.RecordSuccess();
        }

        private async Task UpdateUIWhileWaitingToHearFish(
            IProgress<long> progress, 
            CancellationToken uiUpdateCancelToken)
        {
            // We are waiting a detection from the Ears
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                while (!uiUpdateCancelToken.IsCancellationRequested)
                {
                    progress.Report(stopwatch.ElapsedMilliseconds);
                    await Task.Delay(SECOND / 10, uiUpdateCancelToken);
                }
                uiUpdateCancelToken.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                // Swallow exception; this will hit when a fish is heard and the UI update job is canceled.
                // Explicitly throw and catch instead of not throwing so that the `Task.Delay()` cancellation
                // exception is also caught.
            }
        }

        private async Task HandleNeededAction(NeededAction action)
        {
            switch (action)
            {
                case NeededAction.HearthStone:
                    Stop();
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
