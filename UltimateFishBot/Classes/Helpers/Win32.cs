using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace UltimateFishBot.Helpers
{
    internal class Win32
    {
        private static readonly Random Rnd = new Random();

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CursorInfo
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }

        public enum KeyState
        {
            Keydown = 0,
            Extendedkey = 1,
            Keyup = 2
        };
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        private static extern bool DrawIcon(IntPtr hDc, int x, int y, IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                                         int nWidth, int nHeight, IntPtr hObjectSource,
                                         int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDc, int nWidth,
                                                           int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

        private const uint WmLbuttondown = 513;
        private const uint WmLbuttonup = 514;

        private const uint WmRbuttondown = 516;
        private const uint WmRbuttonup = 517;

        public const int Srccopy = 0x00CC0020; // BitBlt dwRop parameter

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = GetWindowDC(handle);
            // get the size
            Rect windowRect = new Rect();
            GetWindowRect(handle, ref windowRect);

            Rect clientRect = new Rect();
            GetClientRect(handle, out clientRect);


            Point point = new Point { x = 0, y = 0 };
            ClientToScreen(handle, ref point);

            int x = point.x - windowRect.Left;
            int y = point.y - windowRect.Top;
            int width = (clientRect.Right - clientRect.Left);
            int height = (clientRect.Bottom - clientRect.Top);


            // create a device context we can copy to
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            // bitblt over
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, Srccopy);
            // restore selection
            SelectObject(hdcDest, hOld);
            // clean up
            DeleteDC(hdcDest);
            ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Bitmap img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            DeleteObject(hBitmap);
            return img;
        }

        public static Rectangle GetWowRectangle(IntPtr wow)
        {
            Rect win32ApiRect = new Rect();
            GetWindowRect(wow, ref win32ApiRect);
            Rectangle myRect = new Rectangle();
            myRect.X = win32ApiRect.Left;
            myRect.Y = win32ApiRect.Top;
            myRect.Width = (win32ApiRect.Right - win32ApiRect.Left);
            myRect.Height = (win32ApiRect.Bottom - win32ApiRect.Top);
            return myRect;
        }

        private static int GetRandomDelay(int baseDelay = 100)
        {
            return baseDelay + Rnd.Next(-1 * baseDelay / 5, baseDelay / 3);
        }

        public static IntPtr FindWowWindow()
        {
            Process[] processlist = Process.GetProcesses();
            foreach(Process process in processlist)
            {
                if(process.MainWindowTitle.ToUpper().Equals("WORLD OF WARCRAFT".ToUpper()) || process.MainWindowTitle.ToUpper().Equals(@"魔兽世界".ToUpper()))
                {
                    return process.MainWindowHandle;
                }
            }
            return new IntPtr();
        }

        public static Bitmap GetCursorIcon(CursorInfo actualCursor, int width = 35, int height = 35)
        {
            Bitmap actualCursorIcon = null;

            try
            {
                actualCursorIcon = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(actualCursorIcon);
                Win32.DrawIcon(g.GetHdc(), 0, 0, actualCursor.hCursor);
                g.ReleaseHdc();
            }
            catch (Exception) { }

            return actualCursorIcon;
        }

        static public void ActivateWow(IntPtr wow)
        {
            ActivateApp(wow);
        }

        public static void ActivateApp(IntPtr wow)
        {
            SetForegroundWindow(wow);
            //AllowSetForegroundWindow(Process.GetCurrentProcess().Id);
            if (IsIconic(wow))
            {
                ShowWindow(wow, ShowWindowEnum.Restore);
            }
        }

        public static void MoveMouse(int x, int y)
        {
            if (SetCursorPos(x, y))
            {
                _lastX = x;
                _lastY = y;
            }
        }

        public static CursorInfo GetNoFishCursor(IntPtr wow)
        {
            Rectangle woWRect = Win32.GetWowRectangle(wow);
            Win32.MoveMouse((woWRect.X + 10), (woWRect.Y + 45));
            _lastRectX = woWRect.X;
            _lastRectY = woWRect.Y;
            Thread.Sleep(15);
            CursorInfo myInfo = new CursorInfo();
            myInfo.cbSize = Marshal.SizeOf(myInfo);
            GetCursorInfo(out myInfo);
            return myInfo;
        }

        public static CursorInfo GetCurrentCursor()
        {
            CursorInfo myInfo = new CursorInfo();
            myInfo.cbSize = Marshal.SizeOf(myInfo);
            GetCursorInfo(out myInfo);
            return myInfo;
        }

        public static void SendKey(string sKeys)
        {
            if (sKeys != " ")
            {
                if (Properties.Settings.Default.UseAltKey)
                    sKeys = "%(" + sKeys + ")"; // %(X) : Use the alt key
                else
                    sKeys = "{" + sKeys + "}";  // {X} : Avoid UTF-8 errors (é, è, ...)
            }

            SendKeys.Send(sKeys);
        }

        public static void SendMouseClick(IntPtr wow)
        {
            long dWord = MakeDWord((_lastX - _lastRectX), (_lastY - _lastRectY));

            if (Properties.Settings.Default.ShiftLoot)
                SendKeyboardAction(16, KeyState.Keydown);

            SendNotifyMessage(wow, WmRbuttondown, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(GetRandomDelay());
            SendNotifyMessage(wow, WmRbuttonup, (UIntPtr)1, (IntPtr)dWord);

            if (Properties.Settings.Default.ShiftLoot)
                SendKeyboardAction(16, KeyState.Keyup);
        }
        public static void SendMouseDblRightClick(IntPtr wow)
        {
            //long dWord = MakeDWord((LastX - LastRectX), (LastY - LastRectY));
            Rectangle wowRect = Win32.GetWowRectangle(wow);
            long dWord = MakeDWord( (wowRect.Width/2), (wowRect.Height/2) );
            SendNotifyMessage(wow, WmRbuttondown, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(GetRandomDelay());
            SendNotifyMessage(wow, WmRbuttonup, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(GetRandomDelay());
            SendNotifyMessage(wow, WmRbuttondown, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(GetRandomDelay());
            SendNotifyMessage(wow, WmRbuttonup, (UIntPtr)1, (IntPtr)dWord);
        }

        public static bool SendKeyboardAction(Keys key, KeyState state)
        {
            return SendKeyboardAction((byte)key.GetHashCode(), state);
        }

        public static bool SendKeyboardAction(byte key, KeyState state)
        {
            return keybd_event(key, 0, (uint)state, (UIntPtr)0);
        }

        private static long MakeDWord(int loWord, int hiWord)
        {
            return (hiWord << 16) | (loWord & 0xFFFF);
        }

        private static int _lastRectX;
        private static int _lastRectY;

        private static int _lastX;
        private static int _lastY;
    }
}
