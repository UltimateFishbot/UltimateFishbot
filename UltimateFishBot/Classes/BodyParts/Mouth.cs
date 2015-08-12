using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Properties;

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
            if (text == Translate.GetTranslate("manager", "LABEL_PAUSED") || (text == Translate.GetTranslate("manager", "LABEL_STOPPED")))
            {
                m_mainForm.lblStatus.Image = Resources.offline;
            }
            else
            {
                m_mainForm.lblStatus.Image = Resources.online;
            }
        }
    }
}
