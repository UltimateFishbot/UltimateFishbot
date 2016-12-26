namespace UltimateFishBot.Classes
{
    public class FishingStats
    {
        public int totalSuccessFishing { get; private set; }
        public int totalNotFoundFish { get; private set; }
        public int totalNotEaredFish { get; private set; }

        public void Reset()
        {
            totalSuccessFishing = 0;
            totalNotFoundFish = 0;
            totalNotEaredFish = 0;
        }

        public void Looting()
        {
            ++totalSuccessFishing;
        }

        public void CastingOrSearchingForBobber()
        {
            ++totalNotFoundFish;
        }

        public void WaitingForFish()
        {
            ++totalNotEaredFish;
        }

        public int Total()
        {
            return totalSuccessFishing + totalNotFoundFish + totalNotEaredFish;
        }
    }
}
