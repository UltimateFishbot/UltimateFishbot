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

        public Manager(IManagerEventHandler managerEventHandler, IProgress<string> progressHandle)
        {
            m_managerEventHandler = managerEventHandler;

            m_eyes = new Eyes();
            m_hands = new Hands();
            m_ears = new Ears();
            m_mouth = new Mouth(progressHandle);
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
            m_mouth.Say(Translate.GetTranslate("frmMain", "LABEL_STARTED"));
            m_managerEventHandler.Started();
            await RunBot();
        }

        private async Task Resume()
        {
            m_mouth.Say(Translate.GetTranslate("frmMain", "LABEL_RESUMED"));
            m_managerEventHandler.Resumed();
            await RunBot();
        }

        private async Task RunBot()
        {
            m_fishingState = FishingState.Fishing;
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {

                    // We first check if another action is needed, foreach on all NeededAction enum values
                    foreach (NeededAction neededAction in (NeededAction[])Enum.GetValues(typeof(NeededAction)))
                    {
                        if (HasNeededAction(neededAction))
                        {
                            await HandleNeededAction(neededAction, cancellationToken);
                        }
                    }

                    // If no other action required, we can cast !
                    await Fish(cancellationToken);
                }

            }
            catch (TaskCanceledException)
            {
                return;
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void CancelRun()
        {
            if (!IsStoppedOrPaused())
            {
                Debug.Assert(_cancellationTokenSource != null);
                _cancellationTokenSource.Cancel();
            }
        }

        private void Pause()
        {
            CancelRun();
            m_fishingState = FishingState.Paused;
            m_mouth.Say(Translate.GetTranslate("frmMain", "LABEL_PAUSED"));
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
            CancelRun();
            m_fishingState = FishingState.Stopped;
            m_mouth.Say(Translate.GetTranslate("frmMain", "LABEL_STOPPED"));
            m_managerEventHandler.Stopped();
            m_LureTimer.Enabled = false;
            m_RaftTimer.Enabled = false;
            m_CharmTimer.Enabled = false;
            m_BaitTimer.Enabled = false;
            m_HearthStoneTimer.Enabled = false;
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
            await m_hands.Cast(cancellationToken);

            m_mouth.Say(Translate.GetTranslate("manager", "LABEL_FINDING"));
            bool didFindFish = await m_eyes.LookForBobber(cancellationToken);
            if (!didFindFish)
            {
                m_fishingStats.RecordBobberNotFound();
                return;
            }

            // Update UI with wait status            
            var uiUpdateCancelTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var uiUpdateCancelToken = uiUpdateCancelTokenSource.Token;
            var progress = new Progress<long>(msecs =>
            {
                if (!uiUpdateCancelToken.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    m_mouth.Say(Translate.GetTranslate(
                        "manager",
                        "LABEL_WAITING",
                        msecs / SECOND,
                        Properties.Settings.Default.FishWait / SECOND));
                }
            });
            var uiUpdateTask = Task.Run(
                async () => await UpdateUIWhileWaitingToHearFish(progress, uiUpdateCancelToken),
                uiUpdateCancelToken);

            bool fishHeard = await m_ears.Listen(
                Properties.Settings.Default.FishWait,
                cancellationToken);
            uiUpdateCancelTokenSource.Cancel();
            try
            {
                uiUpdateTask.GetAwaiter().GetResult(); // Wait & Unwrap
                // https://github.com/StephenCleary/AsyncEx/blob/dc54d22b06566c76db23af06afcd0727cac625ef/Source/Nito.AsyncEx%20(NET45%2C%20Win8%2C%20WP8%2C%20WPA81)/Synchronous/TaskExtensions.cs#L18
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                uiUpdateCancelTokenSource.Dispose();
            }

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
            while (!uiUpdateCancelToken.IsCancellationRequested)
            {
                progress.Report(stopwatch.ElapsedMilliseconds);
                await Task.Delay(SECOND / 10, uiUpdateCancelToken);
            }
            uiUpdateCancelToken.ThrowIfCancellationRequested();
        }

        private async Task HandleNeededAction(NeededAction action, CancellationToken cancellationToken)
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
                    await m_hands.DoAction(action, m_mouth, cancellationToken);
                    break;
                case NeededAction.AntiAfkMove:
                    await m_legs.DoMovement(t2s, cancellationToken);
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
