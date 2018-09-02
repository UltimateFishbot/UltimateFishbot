namespace UltimateFishBot
{
    public interface IManagerEventHandler
    {
        void Started();
        void Stopped();
        void Resumed();
        void Paused();
    }
}