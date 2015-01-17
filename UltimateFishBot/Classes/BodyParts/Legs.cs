using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            FRONT_BACK  = 0,
            LEFT_RIGHT  = 1,
            JUMP        = 2
        }

        public void DoMovement()
        {
            switch ((Path)Properties.Settings.Default.AntiAfkMoves)
            {
                case Path.FRONT_BACK:
                    MovePath(new Keys[] { Keys.Up, Keys.Down });
                    break;
                case Path.LEFT_RIGHT:
                    MovePath(new Keys[] { Keys.Left, Keys.Right });
                    break;
                case Path.JUMP:
                    MovePath(new Keys[] { Keys.Space });
                    break;
                default:
                    MovePath(new Keys[] { Keys.Left, Keys.Right });
                    break;
            }
        }

        private void MovePath(Keys[] moves)
        {
            foreach (Keys move in moves)
            {
                SingleMove(move);
                Thread.Sleep(250);
            }
        }

        private void SingleMove(Keys move)
        {
            Win32.SendKeyboardAction(move, Win32.keyState.KEYDOWN);
            Thread.Sleep(250);
            Win32.SendKeyboardAction(move, Win32.keyState.KEYUP);
        }
    }
}
