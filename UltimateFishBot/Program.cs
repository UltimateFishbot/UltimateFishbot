using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Serilog;
using UltimateFishBot.Forms;

namespace UltimateFishBot
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Console.Out.WriteLine("Hash dodge");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.File("ufb.log")
                .WriteTo.Trace()
                .CreateLogger();
            Log.Information("UltimateFishBot Started");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
 
        }
    }
}
