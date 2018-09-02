using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFBLauncher
{
    class Program
    {
        private const string ExeExt = ".exe";
        private const string DefaultName = "UltimateFishBot.exe";

        static void Main(string[] args)
        {
            var lastName = Properties.Settings.Default.UFBName;

            if (!RunBotSecretly(lastName)) RunBotSecretly(DefaultName);
        }

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);

        static bool RunBotSecretly(string name)
        {
            var currentPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), name);
            var ufbExeFile = new FileInfo(currentPath);
            if (ufbExeFile.Exists)
            {
                var newName = StringUtils.RandomString(12) + ExeExt;
                var newPath = Path.Combine(Path.GetDirectoryName(currentPath), newName);
                ufbExeFile.MoveTo(newPath);
                Properties.Settings.Default.UFBName = newName;
                Properties.Settings.Default.Save();
                var p = Process.Start(ufbExeFile.FullName);
                Thread.Sleep(1500);
                SetWindowText(p.MainWindowHandle, newName);
                return true;
            }
            return false;
        }
    }
}
