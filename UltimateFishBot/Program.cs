using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Forms;

namespace UltimateFishBot
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.Startup >= 3)
                if (new frmCode().ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

            ++Properties.Settings.Default.Startup;
            Properties.Settings.Default.Save();

            Application.Run(new frmMain());
        }
    }
}
