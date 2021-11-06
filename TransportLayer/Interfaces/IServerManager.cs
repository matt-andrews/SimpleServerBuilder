namespace TransportLayer.Interfaces
{
    public interface IServerManager : ICommon
    {
        void Stop();
        void SendBroadcast(string data);
    }
}
