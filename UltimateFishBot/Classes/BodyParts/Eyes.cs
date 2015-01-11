using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using UltimateFishBot.Classes.Helpers;

namespace UltimateFishBot.Classes.BodyParts
{
    class Eyes
    {
        private Manager m_manager;
        private BackgroundWorker m_backgroundWorker;

        public Eyes(Manager manager)
        {
            m_manager = manager;

            m_backgroundWorker = new BackgroundWorker();
            m_backgroundWorker.WorkerSupportsCancellation = true;
            m_backgroundWorker.DoWork += new DoWorkEventHandler(EyeProcess_DoWork);
            m_backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EyeProcess_RunWorkerCompleted);
        }

        public void StartLooking()
        {
            if (m_backgroundWorker.IsBusy)
                return;

            m_manager.SetActualState(Manager.FishingState.SearchingForBobber);
            m_backgroundWorker.RunWorkerAsync();
        }

        private void EyeProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            LookForBobber();
        }

        private void EyeProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // If not found, exception is sent...
                m_manager.SetActualState(Manager.FishingState.Idle);
                return;
            }

            //... if no exception, we found a fish !
            m_manager.SetActualState(Manager.FishingState.WaitingForFish);
        }

        private void LookForBobber()
        {
            Win32.CursorInfo noFishCursor = Win32.GetNoFishCursor();
            Win32.CursorInfo actualCursor = noFishCursor;

            Rectangle wowRectangle = Win32.GetWowRectangle();

            int xPosMin = wowRectangle.Width / 4;
            int xPosMax = xPosMin * 3;
            int yPosMin = wowRectangle.Height / 4;
            int yPosMax = yPosMin * 3;

            int XPOSSTEP = (int)((xPosMax - xPosMin) / Properties.Settings.Default.ScanningSteps);
            int YPOSSTEP = (int)((yPosMax - yPosMin) / Properties.Settings.Default.ScanningSteps);
            int XOFFSET  = (int)(XPOSSTEP / Properties.Settings.Default.ScanningRetries);

            for (int tryCount = 0; tryCount < Properties.Settings.Default.ScanningRetries; ++tryCount)
            {
                for (int x = (int)(xPosMin + (XOFFSET * tryCount)); x < xPosMax; x += XPOSSTEP)
                {
                    for (int y = yPosMin; y < yPosMax; y += YPOSSTEP)
                    {
                        Win32.MoveMouse(wowRectangle.X + x, wowRectangle.Y + y);

                        // Sleep (give the OS a chance to change the cursor)
                        Thread.Sleep(Properties.Settings.Default.ScanningDelay);

                        actualCursor = Win32.GetCurrentCursor();

                        if (actualCursor.flags == noFishCursor.flags &&
                            actualCursor.hCursor == noFishCursor.hCursor)
                            continue;

                        // Compare the actual icon with our fishIcon if user want it
                        if (Properties.Settings.Default.CheckCursor)
                            if (!ImageCompare(Win32.GetCursorIcon(actualCursor), Properties.Resources.fishIcon35x35))
                                continue;

                        // We found a fish !
                        return;
                    }
                }
            }

            throw new Exception("Fish not found"); // Will be catch in Manager:EyeProcess_RunWorkerCompleted
        }

        private bool ImageCompare(Bitmap firstImage, Bitmap secondImage)
        {
            if (firstImage.Width != secondImage.Width || firstImage.Height != secondImage.Height)
                return false;

            string firstPixel = "";
            string secondPixel = "";

            for (int i = 0; i < firstImage.Width; i++)
            {
                for (int j = 0; j < firstImage.Height; j++)
                {
                    firstPixel = firstImage.GetPixel(i, j).ToString();
                    secondPixel = secondImage.GetPixel(i, j).ToString();
                    if (firstPixel != secondPixel)
                        return false;
                }
            }

            return true;
        }
    }
}
