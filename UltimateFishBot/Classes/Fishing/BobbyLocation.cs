using UltimateFishBot.Helpers;

namespace UltimateFishBot.Fishing
{
    internal class BobbyLocation
    {
        public int X { get; }

        public int Y { get; }

        public int Hits { get; set; } = 0;

        public BobbyLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public BobbyLocation(Win32.Point point)
        {
            X = point.x;
            Y = point.y;
        }

        public Win32.Point ToWin32Point()
        {
            return new Win32.Point {x = X, y = Y};
        }
    }
}
