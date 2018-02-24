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
            JUMP       = 2
        }

        public async Task DoMovement(T2S t2s, CancellationToken cancellationToken)
        {
            switch ((Path)Properties.Settings.Default.AntiAfkMoves)
            {
                case Path.FRONT_BACK:
                    await MovePath(new Keys[] { Keys.Up, Keys.Down }, cancellationToken);
                    break;
                case Path.LEFT_RIGHT:
                    await MovePath(new Keys[] { Keys.Left, Keys.Right }, cancellationToken);
                    break;
                case Path.JUMP:
                    await MovePath(new Keys[] { Keys.Space }, cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                    break;
                default:
                    await MovePath(new Keys[] { Keys.Left, Keys.Right }, cancellationToken);
                    break;
            }
            if (t2s != null)
                t2s.Say("Anti A F K");
        }

        private async Task MovePath(Keys[] moves, CancellationToken cancellationToken)
        {
            foreach (Keys move in moves)
            {
                await SingleMove(move, cancellationToken);
                await Task.Delay(250, cancellationToken);
            }
        }

        private async Task SingleMove(Keys move, CancellationToken cancellationToken)
        {
            Win32.SendKeyboardAction(move, Win32.keyState.KEYDOWN);
            await Task.Delay(250, cancellationToken);
            Win32.SendKeyboardAction(move, Win32.keyState.KEYUP);
        }
    }
}
