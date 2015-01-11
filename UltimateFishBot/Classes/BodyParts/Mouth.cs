using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateFishBot.Classes.BodyParts
{
    class Mouth
    {
        private frmMain m_mainForm;

        public Mouth(frmMain mainForm)
        {
            m_mainForm = mainForm;
        }

        public void Say(string text)
        {
            m_mainForm.lblStatus.Text = text;
        }
    }
}
