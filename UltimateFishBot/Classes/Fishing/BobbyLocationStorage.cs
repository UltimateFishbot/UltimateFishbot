using UltimateFishBot.Collections;

namespace UltimateFishBot.Fishing
{
    internal class BobbyLocationStorage : LimitedCollection<BobbyLocation>
    {
        public BobbyLocationStorage(int size) : base(size)
        {
        }

        public BobbyLocation Add(int x, int y)
        {
            var loc = new BobbyLocation(x, y);
            base.Add(loc);
            return loc;
        }

        protected override int FindLeastImportant()
        {
            var minIndx = 0;
            var minHits = int.MaxValue;
            for (int i = 0; i < Storage.Length; i++)
            {
                if (Storage[i] == null)
                {
                    return i;
                }
                if (Storage[i].Hits < minHits)
                {
                    minIndx = i;
                    minHits = Storage[i].Hits;
                }
            }
            return minIndx;
        }
    }
}
