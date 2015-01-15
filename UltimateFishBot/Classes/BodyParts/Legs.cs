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

        private enum Movement
        {
            FRONT   = 0,
            BACK    = 1,
            LEFT    = 2,
            RIGHT   = 3,
            JUMP    = 4
        }

        public void DoMovement()
        {
            switch ((Path)Properties.Settings.Default.AntiAfkMoves)
            {
                case Path.FRONT_BACK:
                    MovePath(new Movement[] { Movement.FRONT, Movement.BACK });
                    break;
                case Path.LEFT_RIGHT:
                    MovePath(new Movement[] { Movement.LEFT, Movement.RIGHT });
                    break;
                case Path.JUMP:
                    MovePath(new Movement[] { Movement.JUMP });
                    break;
                default:
                    MovePath(new Movement[] { Movement.LEFT, Movement.RIGHT });
                    break;
            }
        }

        private void MovePath(Movement[] moves)
        {
            foreach (Movement move in moves)
            {
                SingleMove(move);
                Thread.Sleep(250);
            }
        }

        private void SingleMove(Movement move)
        {
            byte key = GetKeyFromMovement(move);

            if (key == 0)
                return;

            Win32.SendKeyboardAction(key, Win32.keyState.KEYDOWN);
            Thread.Sleep(250);
            Win32.SendKeyboardAction(key, Win32.keyState.KEYUP);
        }

        private byte GetKeyFromMovement(Movement move)
        {
            switch (move)
            {
                case Movement.FRONT:    return 0x26;
                case Movement.BACK:     return 0x28;
                case Movement.LEFT:     return 0x25;
                case Movement.RIGHT:    return 0x27;
                case Movement.JUMP:     return 0x20;
                default:                return 0;
            }
        }
    }
}
