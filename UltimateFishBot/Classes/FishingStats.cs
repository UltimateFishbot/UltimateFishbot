namespace UltimateFishBot.Classes
{
    public class FishingStats
    {
        public int totalSuccessFishing { get; set; }
        public int totalNotFoundFish { get; set; }
        public int totalNotEaredFish { get; set; }

        public void Reset()
        {
            totalSuccessFishing = 0;
            totalNotFoundFish = 0;
            totalNotEaredFish = 0;
        }

        public int Total()
        {
            return totalSuccessFishing + totalNotFoundFish + totalNotEaredFish;
        }
    }
}
