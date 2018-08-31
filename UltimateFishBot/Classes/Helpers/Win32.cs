using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace UltimateFishBot.Classes.Helpers
{
    class Win32
    {
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

        public enum keyState
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 1,
            KEYUP = 2
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
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        private static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                                         int nWidth, int nHeight, IntPtr hObjectSource,
                                         int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                                                           int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        private const uint WM_LBUTTONDOWN = 513;
        private const uint WM_LBUTTONUP = 514;

        private const uint WM_RBUTTONDOWN = 516;
        private const uint WM_RBUTTONUP = 517;

        public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

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
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, SRCCOPY);
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

        public static Rectangle GetWowRectangle(IntPtr Wow)
        {
            Rect Win32ApiRect = new Rect();
            GetWindowRect(Wow, ref Win32ApiRect);
            Rectangle myRect = new Rectangle();
            myRect.X = Win32ApiRect.Left;
            myRect.Y = Win32ApiRect.Top;
            myRect.Width = (Win32ApiRect.Right - Win32ApiRect.Left);
            myRect.Height = (Win32ApiRect.Bottom - Win32ApiRect.Top);
            return myRect;
        }

        public static IntPtr FindWowWindow()
        {
            Process[] processlist = Process.GetProcesses();
            foreach(Process process in processlist)
            {
                if(process.MainWindowTitle.ToUpper().Equals("WORLD OF WARCRAFT"))
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

        static public void ActivateWow(IntPtr Wow)
        {
            ActivateApp(Wow);
        }

        public static void ActivateApp(IntPtr Wow)
        {
            SetForegroundWindow(Wow);
            //AllowSetForegroundWindow(Process.GetCurrentProcess().Id);
            if (IsIconic(Wow))
            {
                ShowWindow(Wow, ShowWindowEnum.Restore);
            }
        }

        public static void MoveMouse(int x, int y)
        {
            if (SetCursorPos(x, y))
            {
                LastX = x;
                LastY = y;
            }
        }

        public static CursorInfo GetNoFishCursor(IntPtr Wow)
        {
            Rectangle WoWRect = Win32.GetWowRectangle(Wow);
            Win32.MoveMouse((WoWRect.X + 10), (WoWRect.Y + 45));
            LastRectX = WoWRect.X;
            LastRectY = WoWRect.Y;
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

        public static void SendMouseClick(IntPtr Wow)
        {
            long dWord = MakeDWord((LastX - LastRectX), (LastY - LastRectY));

            if (Properties.Settings.Default.ShiftLoot)
                SendKeyboardAction(16, keyState.KEYDOWN);

            SendNotifyMessage(Wow, WM_RBUTTONDOWN, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(100);
            SendNotifyMessage(Wow, WM_RBUTTONUP, (UIntPtr)1, (IntPtr)dWord);

            if (Properties.Settings.Default.ShiftLoot)
                SendKeyboardAction(16, keyState.KEYUP);
        }
        public static void SendMouseDblRightClick(IntPtr Wow)
        {
            //long dWord = MakeDWord((LastX - LastRectX), (LastY - LastRectY));
            Rectangle wowRect = Win32.GetWowRectangle(Wow);
            long dWord = MakeDWord( (wowRect.Width/2), (wowRect.Height/2) );
            SendNotifyMessage(Wow, WM_RBUTTONDOWN, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(100);
            SendNotifyMessage(Wow, WM_RBUTTONUP, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(100);
            SendNotifyMessage(Wow, WM_RBUTTONDOWN, (UIntPtr)1, (IntPtr)dWord);
            Thread.Sleep(100);
            SendNotifyMessage(Wow, WM_RBUTTONUP, (UIntPtr)1, (IntPtr)dWord);
        }

        public static bool SendKeyboardAction(Keys key, keyState state)
        {
            return SendKeyboardAction((byte)key.GetHashCode(), state);
        }

        public static bool SendKeyboardAction(byte key, keyState state)
        {
            return keybd_event(key, 0, (uint)state, (UIntPtr)0);
        }

        private static long MakeDWord(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xFFFF);
        }

        private static int LastRectX;
        private static int LastRectY;

        private static int LastX;
        private static int LastY;
    }
}
