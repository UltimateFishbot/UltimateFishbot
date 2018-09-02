using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UltimateFishBot.Helpers;

namespace UltimateFishBot.BodyParts
{
    internal class Legs
    {
        private enum Path
        {
            FrontBack = 0,
            LeftRight = 1,
            Jump       = 2
        }

        public async Task DoMovement(T2S t2S, CancellationToken cancellationToken)
        {
            switch ((Path)Properties.Settings.Default.AntiAfkMoves)
            {
                case Path.FrontBack:
                    await MovePath(new[] { Keys.Up, Keys.Down }, cancellationToken);
                    break;
                case Path.LeftRight:
                    await MovePath(new[] { Keys.Left, Keys.Right }, cancellationToken);
                    break;
                case Path.Jump:
                    await MovePath(new[] { Keys.Space }, cancellationToken);
                    await Task.Delay(500, cancellationToken);
                    break;
                default:
                    await MovePath(new[] { Keys.Left, Keys.Right }, cancellationToken);
                    break;
            }
            t2S?.Say("Anti A F K");
        }

        private async Task MovePath(Keys[] moves, CancellationToken cancellationToken)
        {
            foreach (var move in moves)
            {
                await SingleMove(move, cancellationToken);
                await Task.Delay(new Random().Next(100,500), cancellationToken);
            }
        }

        private static async Task SingleMove(Keys move, CancellationToken cancellationToken)
        {
            Win32.SendKeyboardAction(move, Win32.KeyState.Keydown);
            await Task.Delay(new Random().Next(100, 250), cancellationToken);
            Win32.SendKeyboardAction(move, Win32.KeyState.Keyup);
        }
    }
}
