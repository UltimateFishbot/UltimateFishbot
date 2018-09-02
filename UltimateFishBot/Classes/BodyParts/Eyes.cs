using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Serilog;
using UltimateFishBot.Extensions;
using UltimateFishBot.Fishing;
using UltimateFishBot.Helpers;

namespace UltimateFishBot.BodyParts
{
    internal class Eyes
    {
        private Win32.CursorInfo _noFishCursor;
        private IntPtr _wow;
        private Bitmap _capturedCursorIcon;
        private Bitmap _background;
        private Rectangle _wowRectangle;

        private int _aScanningSteps;
        private int _aScanningDelay;

        public Eyes(IntPtr wowWindow)  {
            SetWow(wowWindow);
        }

        public void SetWow(IntPtr wowWindow) {
            _wow = wowWindow;
            _noFishCursor = Win32.GetNoFishCursor(_wow);
            _wowRectangle = Win32.GetWowRectangle(_wow);
            if (System.IO.File.Exists("capturedcursor.bmp")) {
                _capturedCursorIcon = new Bitmap("capturedcursor.bmp", true);
            }

        }

        // capture in grayscale
        public void UpdateBackground() {
            _background = new Grayscale(0.3725, 0.6154, 0.0121).Apply(Win32.CaptureWindow(_wow));
            _background = new Pixellate().Apply(_background);
        }

        public async Task<BobbyLocation> LookForBobber(BotSession session, CancellationToken cancellationToken)
        {
            Win32.Rect scanArea;
            if (!Properties.Settings.Default.customScanArea) {
                scanArea.Left = _wowRectangle.X + _wowRectangle.Width / 5;
                scanArea.Right = _wowRectangle.X + _wowRectangle.Width / 5 * 4;
                scanArea.Top = _wowRectangle.Y + _wowRectangle.Height / 4;
                scanArea.Bottom = _wowRectangle.Y + _wowRectangle.Height / 4 * 3;
                //Log.Information("Using default area");
            } else {
                scanArea.Left = Properties.Settings.Default.minScanXY.X;
                scanArea.Top = Properties.Settings.Default.minScanXY.Y;
                scanArea.Right = Properties.Settings.Default.maxScanXY.X;
                scanArea.Bottom = Properties.Settings.Default.maxScanXY.Y;
                //Log.Information("Using custom area");
            }
            Log.Information($"Scanning area: {scanArea.Left} , {scanArea.Top} , {scanArea.Right} , {scanArea.Bottom} cs: {session.BobbyLocations.Count()}");
            
            foreach (var dp in PointOfScreenDifferences()) {
                if (await MoveMouseAndCheckCursor(dp.X, dp.X, cancellationToken,2)) {
                    Log.Information("Bobber imagescan hit. ({bx},{by})", dp.X, dp.X);
                    return dp;
                }
            }
            
            // utilize previous hits
            foreach (var location in session.BobbyLocations) {
                // do something with item.Key and item.Value
                if (await MoveMouseAndCheckCursor(location.X, location.Y, cancellationToken,2))
                {
                    location.Hits++;
                    Log.Information("Bobber position cache hit. ({bx},{by})", location.X, location.Y);
                    return location;
                }
            }
            
            var rnd = new Random();
            BobbyLocation loc;
            _aScanningSteps = rnd.Next(Properties.Settings.Default.ScanningStepsLow, Properties.Settings.Default.ScanningStepsHigh);
            if (Properties.Settings.Default.AlternativeRoute) {
                loc = await LookForBobberSpiralImpl(session, scanArea, _aScanningSteps, Properties.Settings.Default.ScanningRetries, cancellationToken);
            } else {
                loc = await LookForBobberImpl(session, scanArea, _aScanningSteps, Properties.Settings.Default.ScanningRetries, cancellationToken);
            }

            Log.Information("Bobber scan finished. ({bx},{by})", loc?.X, loc?.Y);
            return loc;

        }

        private IEnumerable<BobbyLocation> PointOfScreenDifferences()
        {
            var castbmp = Win32.CaptureWindow(_wow);
            var processingFilter = new FiltersSequence
            {
                new Grayscale(0.3725, 0.6154, 0.0121),
                new Pixellate(),
                new Difference(_background),
                new Threshold(15),
                new Erosion()
            };

            var blobCounter = new BlobCounter();
            blobCounter.ProcessImage(processingFilter.Apply(castbmp));

            var brl = blobCounter.GetObjectsRectangles();
            Log.Information("Bobber imagescan brl: {brl}", brl.Length);
            var sdl = new List<BobbyLocation>();
            foreach (var br in brl) {
                var pt = new Win32.Point { x = (br.Left + br.Left + br.Right) * 4 / 12, y = (br.Top+br.Bottom+br.Bottom)*4/12 };
                Win32.ClientToScreen(_wow, ref pt);
                if (br.Right - br.Left>9&& br.Bottom - br.Top>9) { 
//                    Win32.Point pt = new Win32.Point { x= wowRectangle.X+(br.Left + br.Right) / 2, y= wowRectangle.Y+(br.Top+br.Bottom)/2 };
                    Log.Information("Bobber imagescan br: {bx},{by} - {w},{h}", pt.x,pt.y, br.Right-br.Left,br.Bottom-br.Top);
                    sdl.Add(new BobbyLocation(pt));
//                } else {
//                    Log.Information("Bobber imagescan ignore br: {bx},{by} - {w},{h}", pt.x,pt.y, (br.Right-br.Left),(br.Bottom-br.Top));
                }
            }
            // debug
            /*
            BitmapExt bmpDst = new BitmapExt(castbmp);
            using (var g = Graphics.FromImage(bmpDst)) {
                foreach (var br in brl) {
                    if ((br.Right - br.Left) > 11 && (br.Bottom - br.Top) > 11) {
                        g.DrawRectangle(Pens.White, br);
                    }
                }
            }
            bmpDst.Save("sc_"+DateTime.UtcNow.Ticks+".png", ImageFormat.Png);
            */

            return sdl;
        }

        public async Task<bool> SetMouseToBobber(BotSession session, BobbyLocation bobberPos, CancellationToken cancellationToken)  {// move mouse to previous recorded position and check shape
            if (!await MoveMouseAndCheckCursor(bobberPos.X, bobberPos.Y, cancellationToken, 1)) {
                Log.Information("Bobber lost. ({bx},{by})", bobberPos.X, bobberPos.Y);
                const int fixr = 24;
                Win32.Rect scanArea;
                scanArea.Left = bobberPos.X - fixr;
                scanArea.Right = bobberPos.X + fixr;
                scanArea.Top = bobberPos.Y - fixr;
                scanArea.Bottom = bobberPos.Y + fixr;
                // initiate a small-area search for bobber
                var loc = await LookForBobberSpiralImpl(session, scanArea, 4, 1, cancellationToken);
                if (loc != null) {
                    // search was successful
                    Log.Information("Bobber found. ({bx},{by})", loc.X, loc.Y);
                    return true;
                }

                Log.Information("Bobber lost. ({bx},{by})", 0, 0);
                return false;
            }
            return true;
        }


        private async Task<BobbyLocation> LookForBobberImpl(BotSession session, Win32.Rect scanArea, int steps, int retries, CancellationToken cancellationToken) {

            var xposstep = (scanArea.Right - scanArea.Left) / steps;
            var yposstep = (scanArea.Bottom - scanArea.Top) / steps;
            var xoffset = xposstep / retries;

            for (var tryCount = 0; tryCount < retries; ++tryCount) {
                for (var x = scanArea.Left + xoffset * tryCount; x < scanArea.Right; x += xposstep) {
                    for (var y = scanArea.Top; y < scanArea.Bottom; y += yposstep) {
                        if (await MoveMouseAndCheckCursor(x, y, cancellationToken,1))
                        {
                            return session.BobbyLocations.Add(x, y);
                        }
                    }
                }
            }
            return null;
        }

        private async Task<BobbyLocation> LookForBobberSpiralImpl(BotSession session, Win32.Rect scanArea, int steps, int retries, CancellationToken cancellationToken) {

            var xposstep = (scanArea.Right - scanArea.Left) / steps;
            var yposstep = (scanArea.Bottom - scanArea.Top) / steps;
            var xoffset = xposstep / retries;
            var yoffset = yposstep / retries;

            for (var tryCount = 0; tryCount < retries; ++tryCount) {
                var x = (scanArea.Left + scanArea.Right) / 2 + xoffset * tryCount;
                var y = (scanArea.Top + scanArea.Bottom) / 2 + yoffset * tryCount;

                for (var i = 0; i <= 2 * steps; i++) {
                    for (var j = 0; j <= i / 2; j++) {
                        int dx, dy;
                        if (i % 2 == 0) {
                            if (i / 2 % 2 == 0) {
                                dx = xposstep;
                                dy = 0;
                            } else {
                                dx = -xposstep;
                                dy = 0;
                            }
                        } else {
                            if (i / 2 % 2 == 0) {
                                dx = 0;
                                dy = yposstep;
                            } else {
                                dx = 0;
                                dy = -yposstep;
                            }
                        }
                        x += dx;
                        y += dy;
                        if (await MoveMouseAndCheckCursor(x, y, cancellationToken,1)) {
                            return session.BobbyLocations.Add(x, y);
                        }
                    }
                }
            }

            return null;
        }

        private async Task<bool> MoveMouseAndCheckCursor(int x, int y, CancellationToken cancellationToken,int mpy)   {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            Win32.MoveMouse(x, y);

            // Pause (give the OS a chance to change the cursor)
            var rnd = new Random();
            _aScanningDelay = rnd.Next(Properties.Settings.Default.ScanningDelayLow, Properties.Settings.Default.ScanningDelayHigh);
            await Task.Delay(mpy*_aScanningDelay, cancellationToken);

            var actualCursor = Win32.GetCurrentCursor();

            if (actualCursor.flags == _noFishCursor.flags &&
                actualCursor.hCursor == _noFishCursor.hCursor)
                return false;

            // Compare the actual icon with our fishIcon if user want it
            if (Properties.Settings.Default.CheckCursor) { 
                if (Win32.GetCursorIcon(actualCursor).ImageCompare(Properties.Resources.fishIcon35x35)) { 
                    // We found a fish!
                    return true;
                }
                if (_capturedCursorIcon != null && Win32.GetCursorIcon(actualCursor).ImageCompare(_capturedCursorIcon)) {
                    // We found a fish!
                    return true;
                }
                return false;
            }

            return true;
        }

        public void CaptureCursor() {
            var actualCursor = Win32.GetCurrentCursor();
            var cursorIcon = Win32.GetCursorIcon(actualCursor);
            cursorIcon.Save("capturedcursor.bmp");
        }

    }
}