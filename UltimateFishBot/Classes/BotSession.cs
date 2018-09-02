using UltimateFishBot.Fishing;

namespace UltimateFishBot
{
    internal class BotSession
    {
        internal BobbyLocationStorage BobbyLocations { get; }

        public BotSession()
        {
            BobbyLocations = new BobbyLocationStorage(25);
        }
    }
}