using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using UltimateFishBot.BodyParts;
using UltimateFishBot.Helpers;

namespace UltimateFishBot
{
    [Flags]
    public enum NeededAction
    {
        None        = 0x00,
        HearthStone = 0x01,
        Lure        = 0x02,
        Charm       = 0x04,
        Raft        = 0x08,
        Bait        = 0x10,
        AntiAfkMove = 0x20
    }
}
