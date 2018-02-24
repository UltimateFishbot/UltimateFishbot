namespace UltimateFishBot.Classes
{
    public class FishingStats
    {
        public int totalSuccessFishing { get; private set; }
        public int totalNotFoundFish { get; private set; }
        public int totalNotHeardFish { get; private set; }

        public void Reset()
        {
            totalSuccessFishing = 0;
            totalNotFoundFish   = 0;
            totalNotHeardFish   = 0;
        }

        public void RecordSuccess()
        {
            ++totalSuccessFishing;
        }

        public void RecordBobberNotFound()
        {
            ++totalNotFoundFish;
        }

        public void RecordNotHeard()
        {
            ++totalNotHeardFish;
        }

        public int Total()
        {
            return totalSuccessFishing + totalNotFoundFish + totalNotHeardFish;
        }
    }
}
