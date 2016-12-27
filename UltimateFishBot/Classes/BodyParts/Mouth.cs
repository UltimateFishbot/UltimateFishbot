using System;
using System.Speech.Synthesis;
using UltimateFishBot.Properties;

namespace UltimateFishBot.Classes.BodyParts
{
    class Mouth
    {
        private IProgress<string> m_progressHandle;
        T2S t2s = new T2S();

        public Mouth(IProgress<string> progressHandle)
        {
            m_progressHandle = progressHandle;
        }

        public void Say(string text)
        {
            m_progressHandle.Report(text);
            t2s.Say(text);

        }

    }
    class T2S
    {
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        bool uset2s;
        string lastMessage;

        public T2S()
        {
            uset2s = Properties.Settings.Default.Txt2speech;
            synthesizer.Volume = 60;  // 0...100
            synthesizer.Rate = 1;     // -10...10
        }

        public void Say(string text)
        {
            //Debug code
            //System.Console.WriteLine("Use T2S: " + uset2s);
            //System.Console.WriteLine("Previous message: " + lastMessage);
            //System.Console.WriteLine("Current message: " + text);
            //System.Console.WriteLine("Synthesizer ready: " + (synthesizer.State == SynthesizerState.Ready));

            // Say asynchronous text through Text 2 Speech synthesizer
            if (uset2s && (lastMessage != text) && (synthesizer.State == SynthesizerState.Ready))
            {
                synthesizer.SpeakAsync(text);
                lastMessage = text;
            }
        }
    }
}
