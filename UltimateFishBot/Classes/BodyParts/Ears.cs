using CoreAudioApi;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UltimateFishBot.Collections;

namespace UltimateFishBot.BodyParts
{
    internal class Ears
    {
        private MMDevice _sndDevice;
        private readonly LimitedCollection<int> _volumeQueue;
        private int _tickrate = 50; //ms pause between sound checks

        private const int MaxVolumeQueueLength = 5;

        public Ears()
        {
            _volumeQueue = new LimitedCollection<int>(MaxVolumeQueueLength) {0};
        }

        public async Task<bool> Listen(int millisecondsToListen, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sndDevEnum = new MMDeviceEnumerator();
            _sndDevice = Properties.Settings.Default.AudioDevice != ""
                ? sndDevEnum.GetDevice(Properties.Settings.Default.AudioDevice)
                : sndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            Func<bool> heardFish;
            if (Properties.Settings.Default.AverageSound)
                heardFish = ListenTimerTickAvg;
            else
                heardFish = ListenTimerTick;

            while (stopwatch.ElapsedMilliseconds <= millisecondsToListen) {
                await Task.Delay(_tickrate, cancellationToken);
                if (heardFish()) {
                    return true;
                }
            }
            return false;
        }

        private bool ListenTimerTick() {
            // Get the current level
            var currentVolumnLevel = (int)(_sndDevice.AudioMeterInformation.MasterPeakValue * 100);

            if (currentVolumnLevel >= Properties.Settings.Default.SplashLimit)
                return true;

            return false;
        }

        private bool ListenTimerTickAvg()
        {
            // Get the current level
            var currentVolumnLevel = (int)(_sndDevice.AudioMeterInformation.MasterPeakValue * 100);
            var avgVol = GetAverageVolume();
            var hear = false;

            // Determine if the current level is high enough to be a fish
            if (currentVolumnLevel - avgVol >= Properties.Settings.Default.SplashLimit) {
                Serilog.Log.Information("Hear: {av},{cvl},{queue}", avgVol, currentVolumnLevel, _volumeQueue);
                hear = true;
            }

            // Keep a running queue of the last X sounds as a reference point
            _volumeQueue.Add(currentVolumnLevel);
            return hear;

        }

        private int GetAverageVolume()
        {
            return (int)_volumeQueue.Average();
        }
    }
}
