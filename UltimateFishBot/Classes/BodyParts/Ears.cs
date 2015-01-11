using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateFishBot.Classes.BodyParts
{
    class Ears
    {
        private MMDevice SndDevice;
        private Timer listenTimer;
        private Queue<int> m_volumeQueue;
        private Manager m_manager;

        private const int MAX_VOLUME_QUEUE_LENGTH = 5;

        public Ears(Manager manager)
        {
            m_manager       = manager;
            m_volumeQueue   = new Queue<int>();

            MMDeviceEnumerator SndDevEnum = new MMDeviceEnumerator();
            if (Properties.Settings.Default.AudioDevice != "")
                SndDevice = SndDevEnum.GetDevice(Properties.Settings.Default.AudioDevice);
            else
                SndDevice = SndDevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            listenTimer = new Timer();
            listenTimer.Interval = 500;
            listenTimer.Tick += new EventHandler(ListenTimerTick);
            listenTimer.Enabled = true;
        }

        private void ListenTimerTick(Object myObject, EventArgs myEventArgs)
        {
            // Get the current level
            int currentVolumnLevel = (int)(SndDevice.AudioMeterInformation.MasterPeakValue * 100);
            m_volumeQueue.Enqueue(currentVolumnLevel);

            // Keep a running queue of the last X sounds as a reference point
            if (m_volumeQueue.Count >= MAX_VOLUME_QUEUE_LENGTH)
                m_volumeQueue.Dequeue();

            // Determine if the current level is high enough to be a fish
            if (currentVolumnLevel - GetAverageVolume() >= Properties.Settings.Default.SplashLimit)
                m_manager.HearFish();
        }

        private int GetAverageVolume()
        {
            return (int)m_volumeQueue.Average();
        }
    }
}
