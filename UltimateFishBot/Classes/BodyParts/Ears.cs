using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateFishBot.Classes.BodyParts
{
    class Ears
    {
        private MMDevice SndDevice;
        private Queue<int> m_volumeQueue;
        private Manager m_manager;
        private int tickrate = 100; //ms pause between sound checks

        private const int MAX_VOLUME_QUEUE_LENGTH = 5;

        public Ears(Manager manager)
        {
            m_manager = manager;
            m_volumeQueue = new Queue<int>();
        }

        public async Task Listen(CancellationToken cancellationToken)
        {
            MMDeviceEnumerator SndDevEnum = new MMDeviceEnumerator();
            if (Properties.Settings.Default.AudioDevice != "")
                SndDevice = SndDevEnum.GetDevice(Properties.Settings.Default.AudioDevice);
            else
                SndDevice = SndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            while (true)
            {
                await Task.Delay(tickrate, cancellationToken);
                if (Properties.Settings.Default.AverageSound)
                    await ListenTimerTickAvg();
                else
                    await ListenTimerTick();
            }
        }

        private async Task ListenTimerTick()
        {
            // Get the current level
            int currentVolumnLevel = (int)(SndDevice.AudioMeterInformation.MasterPeakValue * tickrate);

            if (currentVolumnLevel >= Properties.Settings.Default.SplashLimit)
                await m_manager.HearFish();

            // Debug code
            //if (m_manager.IsStoppedOrPaused() == false)
            //{
            //    Debug.WriteLine("Average volume: " + avgVol);
            //    Debug.WriteLine("Current volume: " + currentVolumnLevel);
            //    Debug.WriteLine("Queue values: ");
            //    foreach (int v in m_volumeQueue)
            //    {
            //        Debug.WriteLine("> " + v);
            //    }
            //    Debug.WriteLine("Splash limit: " + Properties.Settings.Default.SplashLimit);
            //    Debug.WriteLine("______________________");
            //}
        }

        private async Task ListenTimerTickAvg()
        {
            // Get the current level
            int currentVolumnLevel = (int)(SndDevice.AudioMeterInformation.MasterPeakValue * tickrate);
            m_volumeQueue.Enqueue(currentVolumnLevel);

            // Keep a running queue of the last X sounds as a reference point
            if (m_volumeQueue.Count >= MAX_VOLUME_QUEUE_LENGTH)
                m_volumeQueue.Dequeue();

            // Determine if the current level is high enough to be a fish
            int avgVol = GetAverageVolume();
            if (currentVolumnLevel - avgVol >= Properties.Settings.Default.SplashLimit)
                await m_manager.HearFish();

            // Debug code
            //if (m_manager.IsStoppedOrPaused() == false)
            //{
            //    Debug.WriteLine("Average volume: " + avgVol);
            //    Debug.WriteLine("Current volume: " + currentVolumnLevel);
            //    Debug.WriteLine("Queue values: ");
            //    foreach (int v in m_volumeQueue)
            //    {
            //        Debug.WriteLine("> " + v);
            //    }
            //    Debug.WriteLine("Splash limit: " + Properties.Settings.Default.SplashLimit);
            //    Debug.WriteLine("______________________");
            //}
        }

        private int GetAverageVolume()
        {
            return (int)m_volumeQueue.Average();
        }
    }
}
