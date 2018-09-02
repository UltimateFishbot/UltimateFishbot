using System;

namespace UltimateFishBot.BodyParts
{
    internal class Mouth
    {
        private readonly IProgress<string> _progressHandle;
        private readonly T2S _t2S = new T2S();

        public Mouth(IProgress<string> progressHandle)
        {
            _progressHandle = progressHandle;
        }

        public void Say(string text)
        {
            _progressHandle.Report(text);
            _t2S.Say(text);

        }
    }
}
