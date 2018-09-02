namespace UltimateFishBot
{
    public class FishingStats
    {
        public int TotalSuccessFishing { get; private set; }
        public int TotalNotFoundFish { get; private set; }
        public int TotalNotEaredFish { get; private set; }

        public void Reset()
        {
            TotalSuccessFishing = 0;
            TotalNotFoundFish   = 0;
            TotalNotEaredFish   = 0;
        }

        public void RecordSuccess()
        {
            ++TotalSuccessFishing;
        }

        public void RecordBobberNotFound()
        {
            ++TotalNotFoundFish;
        }

        public void RecordNotHeard()
        {
            ++TotalNotEaredFish;
        }

        public int Total()
        {
            return TotalSuccessFishing + TotalNotFoundFish + TotalNotEaredFish;
        }
    }
}
