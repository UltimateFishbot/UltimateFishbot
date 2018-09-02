using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using UltimateFishBot.BodyParts;
using UltimateFishBot.Helpers;

namespace UltimateFishBot
{
    public class Manager
    {
        private enum FishingState
        {
            Fishing = 3,
            Paused  = 6,
            Stopped = 7
        }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly System.Windows.Forms.Timer _lureTimer;
        private readonly System.Windows.Forms.Timer _hearthStoneTimer;
        private readonly System.Windows.Forms.Timer _raftTimer;
        private readonly System.Windows.Forms.Timer _charmTimer;
        private readonly System.Windows.Forms.Timer _baitTimer;
        private readonly System.Windows.Forms.Timer _antiAfkTimer;

        private readonly IManagerEventHandler _mManagerEventHandler;

        private readonly Eyes _eyes;
        private readonly Hands _hands;
        private readonly Ears _ears;
        private readonly Mouth _mouth;
        private readonly Legs _legs;
        private T2S _t2S;

        private NeededAction _neededActions;
        private FishingState _fishingState;
        private readonly FishingStats _fishingStats;
        private int _fishErrorLength;

        private const int Second = 1000;
        private const int Minute = 60 * Second;

        ///  average
        private int _aFishWait;


        public Manager(IManagerEventHandler managerEventHandler, IProgress<string> progressHandle)
        {
            _mManagerEventHandler    = managerEventHandler;
            var wowWindowPointer = Win32.FindWowWindow();
            while (wowWindowPointer == new IntPtr())
            {
                var result = MessageBox.Show(
                    @"Could not find the the WoW process. Please make sure the game is running.",
                    @"Error - WoW not open", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Cancel)
                    Environment.Exit(1);
                wowWindowPointer = Win32.FindWowWindow();
            }
            _eyes                   = new Eyes(wowWindowPointer);
            _hands                  = new Hands(wowWindowPointer);
            _ears                   = new Ears();
            _mouth                  = new Mouth(progressHandle);
            _legs                   = new Legs();

            _fishingState           = FishingState.Stopped;
            _neededActions          = NeededAction.None;

            _fishingStats           = new FishingStats();
            _fishingStats.Reset();

            _cancellationTokenSource = null;

            _lureTimer = InitializeTimer(LureTimerTick);
            _charmTimer = InitializeTimer(CharmTimerTick);
            _raftTimer = InitializeTimer(RaftTimerTick);
            _baitTimer = InitializeTimer(BaitTimerTick);
            _hearthStoneTimer = InitializeTimer(HearthStoneTimerTick);
            _antiAfkTimer = InitializeTimer(AntiAfkTimerTick);

            ResetTimers();
        }

        private static System.Windows.Forms.Timer InitializeTimer(EventHandler handler)
        {
            var timer = new System.Windows.Forms.Timer {Enabled = false};
            timer.Tick += handler;
            return timer;
        }

        public async Task StartOrResumeOrPause()
        {
            switch (_fishingState)
            {
                case FishingState.Stopped:
                    await RunBotUntilCanceled();
                    break;
                case FishingState.Paused:
                    await Resume();
                    break;
                default:
                    Pause();
                    break;
            }
        }

        private async Task RunBotUntilCanceled()
        {
            var wowWindowPointer = Win32.FindWowWindow(); // update window pointer in case wow started after fishbot or restarted.
            _eyes.SetWow(wowWindowPointer);
            _hands.SetWow(wowWindowPointer);
            ResetTimers();
            EnableTimers();
            _mouth.Say(Translate.GetTranslate("frmMain", "LABEL_STARTED"));
            _mManagerEventHandler.Started();
            await RunBot();
        }

        private async Task Resume()
        {
            _mouth.Say(Translate.GetTranslate("frmMain", "LABEL_RESUMED"));
            _mManagerEventHandler.Resumed();
            await RunBot();
        }

        private async Task RunBot()
        {
            _fishErrorLength = 0;
            _fishingState = FishingState.Fishing;
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            var session = new BotSession();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {

                    // We first check if another action is needed, foreach on all NeededAction enum values
                    foreach (var neededAction in (NeededAction[])Enum.GetValues(typeof(NeededAction)))
                    {
                        if (HasNeededAction(neededAction))
                        {
                            await HandleNeededAction(neededAction, cancellationToken);
                        }
                    }

                    // If no other action required, we can cast !
                    await Fish(session, cancellationToken);
                    if (_fishErrorLength > 10 ) {
                        Stop();
                    }
                }

            }
            catch (TaskCanceledException)
            {
                //ignore
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
            _fishingState = FishingState.Paused;
            _mouth.Say(Translate.GetTranslate("frmMain", "LABEL_PAUSED"));
            _mManagerEventHandler.Paused();
        }

        private void EnableTimers()
        {
            if (Properties.Settings.Default.AutoLure)
            {
                AddNeededAction(NeededAction.Lure);
                _lureTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoCharm)
            {
                AddNeededAction(NeededAction.Charm);
                _charmTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoRaft)
            {
                AddNeededAction(NeededAction.Raft);
                _raftTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoBait)
            {
                AddNeededAction(NeededAction.Bait);
                _baitTimer.Enabled = true;
            }

            if (Properties.Settings.Default.AutoHearth)
                _hearthStoneTimer.Enabled = true;

            if (Properties.Settings.Default.AntiAfk)
                _antiAfkTimer.Enabled = true;
        }

        public void Stop()
        {
            CancelRun();
            _fishingState = FishingState.Stopped;
            _mouth.Say(Translate.GetTranslate("frmMain", "LABEL_STOPPED"));
            _mManagerEventHandler.Stopped();
            _lureTimer.Enabled        = false;
            _raftTimer.Enabled        = false;
            _charmTimer.Enabled       = false;
            _baitTimer.Enabled        = false;
            _hearthStoneTimer.Enabled = false;
        }

        private bool IsStoppedOrPaused()
        {
            return _fishingState == FishingState.Stopped || _fishingState == FishingState.Paused;
        }

        public FishingStats GetFishingStats()
        {
            return _fishingStats;
        }

        public void ResetFishingStats()
        {
            _fishingStats.Reset();
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
            _lureTimer.Interval        = Properties.Settings.Default.LureTime * Minute + 22 * Second;
            _raftTimer.Interval        = Properties.Settings.Default.RaftTime * Minute;
            _charmTimer.Interval       = Properties.Settings.Default.CharmTime * Minute;
            _baitTimer.Interval        = Properties.Settings.Default.BaitTime * Minute;
            _hearthStoneTimer.Interval = Properties.Settings.Default.HearthTime * Minute;
            _antiAfkTimer.Interval     = Properties.Settings.Default.AntiAfkTime * Minute;
        }

        private async Task Fish(BotSession session, CancellationToken cancellationToken)
        {
            _mouth.Say(Translate.GetTranslate("manager", "LABEL_CASTING"));
            _eyes.UpdateBackground();
            await _hands.Cast(cancellationToken);

            _mouth.Say(Translate.GetTranslate("manager", "LABEL_FINDING"));
            // Make bobber found async, so can check fishing sound in parallel, the result only important when we hear fish.
            // The position used for repositioning.
            var eyeCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var eyeCancelToken = eyeCancelTokenSource.Token;
            var eyeTask = Task.Run(async () => await _eyes.LookForBobber(session, eyeCancelToken), eyeCancelToken);

            // Update UI with wait status            
            var uiUpdateCancelTokenSource =  CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var uiUpdateCancelToken = uiUpdateCancelTokenSource.Token;
            var progress = new Progress<long>(msecs =>
            {
                if (!uiUpdateCancelToken.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    _mouth.Say(Translate.GetTranslate(
                        "manager",
                        "LABEL_WAITING",
                        msecs / Second,
                        _aFishWait / Second));
                }
            });
            var uiUpdateTask = Task.Run(
                async () => await UpdateUiWhileWaitingToHearFish(progress, uiUpdateCancelToken),
                uiUpdateCancelToken);

            var rnd = new Random();
            _aFishWait = rnd.Next(Properties.Settings.Default.FishWaitLow, Properties.Settings.Default.FishWaitHigh);
            var fishHeard = await _ears.Listen(
                _aFishWait,
                cancellationToken);
            //Log.Information("Ear result: "+a_FishWait.ToString());

            uiUpdateCancelTokenSource.Cancel();
            try {
                uiUpdateTask.GetAwaiter().GetResult(); // Wait & Unwrap
                // https://github.com/StephenCleary/AsyncEx/blob/dc54d22b06566c76db23af06afcd0727cac625ef/Source/Nito.AsyncEx%20(NET45%2C%20Win8%2C%20WP8%2C%20WPA81)/Synchronous/TaskExtensions.cs#L18
            } catch (TaskCanceledException) {
            } finally {
                uiUpdateCancelTokenSource.Dispose();
            }

            if (!fishHeard) {
                _fishingStats.RecordNotHeard();
                _fishErrorLength++;
                return;
            }

            // We heard the fish, let's check bobbers position
            if (!eyeTask.IsCompleted) {
                // the search is not finished yet, but fish is heard, we have 2 seconds left to find and hook it
                eyeTask.Wait(2000, cancellationToken);
                eyeCancelTokenSource.Cancel();
            }
            eyeCancelTokenSource.Dispose();

            if (eyeTask.IsCompleted) {
                // search is ended what's the result?
                var bobberPos = eyeTask.Result;

                if (bobberPos != null && bobberPos.X != 0 && bobberPos.Y != 0) {
                    // bobber found
                    if (await _eyes.SetMouseToBobber(session, bobberPos, cancellationToken)) {
                        // bobber is still there
                        Log.Information("Bobber databl: ({bx},{by})", bobberPos.X, bobberPos.Y);
                        await _hands.Loot();
                        _mouth.Say(Translate.GetTranslate("manager", "LABEL_HEAR_FISH"));
                        _fishingStats.RecordSuccess();
                        _fishErrorLength = 0;
                        Log.Information("Fish success");
                        return;
                    }
                }
            }
            _fishingStats.RecordBobberNotFound();
            _fishErrorLength++;
        }

        public void CaptureCursor() {
            _eyes.CaptureCursor();
        }


        private async Task UpdateUiWhileWaitingToHearFish(
            IProgress<long> progress, 
            CancellationToken uiUpdateCancelToken)
        {
            // We are waiting a detection from the Ears
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!uiUpdateCancelToken.IsCancellationRequested)
            {
                progress.Report(stopwatch.ElapsedMilliseconds);
                await Task.Delay(Second / 10, uiUpdateCancelToken);
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
                    await _hands.DoAction(action, _mouth, cancellationToken);
                    break;
                case NeededAction.AntiAfkMove:
                    await _legs.DoMovement(_t2S, cancellationToken);
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
            _neededActions |= action;
        }

        private void RemoveNeededAction(NeededAction action)
        {
            _neededActions &= ~action;
        }

        private bool HasNeededAction(NeededAction action)
        {
            return (_neededActions & action) != NeededAction.None;
        }
    }
}
