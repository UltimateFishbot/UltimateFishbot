using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using UltimateFishBot.Classes.Helpers;

namespace UltimateFishBot.Classes.BodyParts
{
    class NoFishFoundException : Exception { }
    class Eyes
    {
        int xPosMin;
        int xPosMax;
        int yPosMin;
        int yPosMax;
        Rectangle wowRectangle;
        private Win32.CursorInfo m_noFishCursor;

        class BobberLocation
        {
            public uint count;
            public Point location;
            public DateTime last;

            public BobberLocation(Point location)
            {
                this.count = 0;
                this.location = location;
                Use();
            }

            public void Use()
            {
                this.count++;
                this.last = DateTime.Now;
                System.Console.Out.WriteLine("BobberLocation: ({0},{1}) x{2}", location.X, location.Y, count);
            }
        };
        List<BobberLocation> listBobberLocations = new List<BobberLocation>();

        public void ClearHistory()
        {
            listBobberLocations.Clear();
            m_noFishCursor = new Win32.CursorInfo();
        }

        public async Task<bool> LookForBobber(CancellationToken cancellationToken)
        {
            wowRectangle = Win32.GetWowRectangle();

            if (!Properties.Settings.Default.customScanArea)
            {
                xPosMin = wowRectangle.Width / 4;
                xPosMax = xPosMin * 3;
                yPosMin = wowRectangle.Height / 4;
                yPosMax = yPosMin * 3;
                System.Console.Out.WriteLine("Using default area");
            }
            else
            {
                xPosMin = Properties.Settings.Default.minScanXY.X;
                yPosMin = Properties.Settings.Default.minScanXY.Y;
                xPosMax = Properties.Settings.Default.maxScanXY.X;
                yPosMax = Properties.Settings.Default.maxScanXY.Y;
                System.Console.Out.WriteLine("Using custom area");
            }
            System.Console.Out.WriteLine("Scanning area: " + xPosMin.ToString() + " , " + yPosMin.ToString() + " , " + xPosMax.ToString() + " , " + yPosMax.ToString() + " , ");
            try
            {
                if (await CheckCursor(cancellationToken))
                {
                    System.Console.Out.WriteLine("BobberLocation move not required");
                    return true;
                }

                if (m_noFishCursor.cbSize == 0)
                {
                    m_noFishCursor = Win32.GetNoFishCursor();
                }

                if (await LookForBobberRecent(cancellationToken))
                    return true;

                if (Properties.Settings.Default.AlternativeRoute)
                    await LookForBobberSpiralImpl(cancellationToken);
                else
                    await LookForBobberImpl(cancellationToken);

                // Found the fish!
                return true;
            }
            catch (NoFishFoundException)
            {
                // Didn't find the fish
                return false;
            }
        }

        private async Task<bool> LookForBobberRecent(CancellationToken cancellationToken)
        {
            foreach (BobberLocation bobberLocation in listBobberLocations)
            {
                if (await MoveMouseAndCheckCursor(bobberLocation.location.X, bobberLocation.location.Y, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task LookForBobberImpl(CancellationToken cancellationToken)
        {

            int XPOSSTEP = (int)((xPosMax - xPosMin) / Properties.Settings.Default.ScanningSteps);
            int YPOSSTEP = (int)((yPosMax - yPosMin) / Properties.Settings.Default.ScanningSteps);
            int XOFFSET  = (int)(XPOSSTEP / Properties.Settings.Default.ScanningRetries);

            if (Properties.Settings.Default.customScanArea)
            {
                for (int tryCount = 0; tryCount < Properties.Settings.Default.ScanningRetries; ++tryCount)
                {
                    for (int x = (int)(xPosMin + (XOFFSET * tryCount)); x < xPosMax; x += XPOSSTEP)
                    {
                        for (int y = yPosMin; y < yPosMax; y += YPOSSTEP)
                        {
                            if (await MoveMouseAndCheckCursor(x, y, cancellationToken))
                                return;
                        }
                    }
                }
            }
            else
            {
                for (int tryCount = 0; tryCount < Properties.Settings.Default.ScanningRetries; ++tryCount)
                {
                    for (int x = (int)(xPosMin + (XOFFSET * tryCount)); x < xPosMax; x += XPOSSTEP)
                    {
                        for (int y = yPosMin; y < yPosMax; y += YPOSSTEP)
                        {
                            if (await MoveMouseAndCheckCursor(wowRectangle.X + x, wowRectangle.Y + y, cancellationToken))
                                return;
                        }
                    }
                }
            }

            throw new NoFishFoundException(); // Will be catch in Manager:EyeProcess_RunWorkerCompleted
        }

        private async Task LookForBobberSpiralImpl(CancellationToken cancellationToken)
        {
            int XPOSSTEP = (int)((xPosMax - xPosMin) / Properties.Settings.Default.ScanningSteps);
            int YPOSSTEP = (int)((yPosMax - yPosMin) / Properties.Settings.Default.ScanningSteps);
            int XOFFSET  = (int)(XPOSSTEP / Properties.Settings.Default.ScanningRetries);
            int YOFFSET  = (int)(YPOSSTEP / Properties.Settings.Default.ScanningRetries);

            if (Properties.Settings.Default.customScanArea)
            {
                for (int tryCount = 0; tryCount < Properties.Settings.Default.ScanningRetries; ++tryCount)
                {
                    int x = (int)((xPosMin + xPosMax) / 2) + XOFFSET * tryCount;
                    int y = (int)((yPosMin + yPosMax) / 2) + YOFFSET * tryCount;

                    for (int i = 0; i <= 2 * Properties.Settings.Default.ScanningSteps; i++)
                    {
                        for (int j = 0; j <= (i / 2); j++)
                        {
                            int dx = 0, dy = 0;

                            if (i % 2 == 0)
                            {
                                if ((i / 2) % 2 == 0)
                                {
                                    dx = XPOSSTEP;
                                    dy = 0;
                                }
                                else
                                {
                                    dx = -XPOSSTEP;
                                    dy = 0;
                                }
                            }
                            else
                            {
                                if ((i / 2) % 2 == 0)
                                {
                                    dx = 0;
                                    dy = YPOSSTEP;
                                }
                                else
                                {
                                    dx = 0;
                                    dy = -YPOSSTEP;
                                }
                            }

                            x += dx;
                            y += dy;

                            if (await MoveMouseAndCheckCursor(x, y, cancellationToken))
                                return;
                        }
                    }
                }
            }
            else
            {
                for (int tryCount = 0; tryCount < Properties.Settings.Default.ScanningRetries; ++tryCount)
                {
                    int x = (int)((xPosMin + xPosMax) / 2) + XOFFSET * tryCount;
                    int y = (int)((yPosMin + yPosMax) / 2) + YOFFSET * tryCount;

                    for (int i = 0; i <= 2 * Properties.Settings.Default.ScanningSteps; i++)
                    {
                        for (int j = 0; j <= (i / 2); j++)
                        {
                            int dx = 0, dy = 0;

                            if (i % 2 == 0)
                            {
                                if ((i / 2) % 2 == 0)
                                {
                                    dx = XPOSSTEP;
                                    dy = 0;
                                }
                                else
                                {
                                    dx = -XPOSSTEP;
                                    dy = 0;
                                }
                            }
                            else
                            {
                                if ((i / 2) % 2 == 0)
                                {
                                    dx = 0;
                                    dy = YPOSSTEP;
                                }
                                else
                                {
                                    dx = 0;
                                    dy = -YPOSSTEP;
                                }
                            }

                            x += dx;
                            y += dy;

                            if (await MoveMouseAndCheckCursor(wowRectangle.X + x, wowRectangle.Y + y, cancellationToken))
                                return;
                        }
                    }
                }
            }

            throw new NoFishFoundException(); // Will be catch in Manager:EyeProcess_RunWorkerCompleted
        }

        private async Task<bool> CheckCursor(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            // Pause (give the OS a chance to change the cursor)
            await Task.Delay(Properties.Settings.Default.ScanningDelay, cancellationToken);

            Win32.CursorInfo actualCursor = Win32.GetCurrentCursor();

            if (actualCursor.flags == m_noFishCursor.flags &&
                actualCursor.hCursor == m_noFishCursor.hCursor)
                return false;

            // Compare the actual icon with our fishIcon if user want it
            if (Properties.Settings.Default.CheckCursor)
                if (!ImageCompare(Win32.GetCursorIcon(actualCursor), Properties.Resources.fishIcon35x35))
                    return false;

            // We found a bobber!
            return true;
        }

        private async Task<bool> MoveMouseAndCheckCursor(int x, int y, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            Win32.MoveMouse(x, y);

            bool found = await CheckCursor(cancellationToken);

            if (found)
            {
                Point point = new Point(x, y);
                BobberLocation bobberLocation;
                int index = listBobberLocations.FindIndex(o => (o.location == point));
                if (index >= 0)
                {
                    bobberLocation = listBobberLocations[index];
                    listBobberLocations.RemoveAt(index);
                    bobberLocation.Use();
                }
                else
                {
                    bobberLocation = new BobberLocation(point);
                }
                listBobberLocations.Insert(0, bobberLocation);
            }

            return found;
        }

        private bool ImageCompare(Bitmap firstImage, Bitmap secondImage)
        {
            if (firstImage.Width != secondImage.Width || firstImage.Height != secondImage.Height)
                return false;

            for (int i = 0; i < firstImage.Width; i++)
                for (int j = 0; j < firstImage.Height; j++)
                    if (firstImage.GetPixel(i, j).ToString() != secondImage.GetPixel(i, j).ToString())
                        return false;

            return true;
        }
    }
}