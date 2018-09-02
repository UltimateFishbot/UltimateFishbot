using System.Speech.Synthesis;

namespace UltimateFishBot.BodyParts
{
    internal class T2S
    {
        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
        private readonly bool _uset2S;
        private string _lastMessage;

        public T2S()
        {
            _uset2S = Properties.Settings.Default.Txt2speech;
            _synthesizer.Volume = 60;  // 0...100
            _synthesizer.Rate   = 1;   // -10...10
        }

        public void Say(string text)
        {
            //Debug code
            //System.Console.WriteLine("Use T2S: " + uset2s);
            //System.Console.WriteLine("Previous message: " + lastMessage);
            //System.Console.WriteLine("Current message: " + text);
            //System.Console.WriteLine("Synthesizer ready: " + (synthesizer.State == SynthesizerState.Ready));

            // Say asynchronous text through Text 2 Speech synthesizer
            if (_uset2S && (_lastMessage != text) && (_synthesizer.State == SynthesizerState.Ready))
            {
                _synthesizer.SpeakAsync(text);
                _lastMessage = text;
            }
        }
    }
}