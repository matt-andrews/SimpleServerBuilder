namespace TransportLayer.Interfaces
{
    public interface IClientManager : ICommon
    {
        TNetPeer FirstPeer();
        void Disconnect();
    }
}
