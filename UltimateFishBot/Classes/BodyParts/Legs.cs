using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Classes.Helpers;

namespace UltimateFishBot.Classes.BodyParts
{
    class Legs
    {
        public enum Path
        {
            FRONT_BACK = 0,
            LEFT_RIGHT = 1,
            JUMP = 2
        }

        public async Task DoMovement(T2S t2s)
        {
            switch ((Path)Properties.Settings.Default.AntiAfkMoves)
            {
                case Path.FRONT_BACK:
                    await MovePath(new Keys[] { Keys.Up, Keys.Down });
                    break;
                case Path.LEFT_RIGHT:
                    await MovePath(new Keys[] { Keys.Left, Keys.Right });
                    break;
                case Path.JUMP:
                    await MovePath(new Keys[] { Keys.Space });
                    break;
                default:
                    await MovePath(new Keys[] { Keys.Left, Keys.Right });
                    break;
            }
            if (t2s != null)
                t2s.Say("Anti A F K");
        }

        private async Task MovePath(Keys[] moves)
        {
            foreach (Keys move in moves)
            {
                await SingleMove(move);
                await Task.Delay(250);
            }
        }

        private async Task SingleMove(Keys move)
        {
            Win32.SendKeyboardAction(move, Win32.keyState.KEYDOWN);
            await Task.Delay(250);
            Win32.SendKeyboardAction(move, Win32.keyState.KEYUP);
        }
    }
}
