namespace TransportLayer.Interfaces
{
    public interface IEventListener
    {
        void OnConnectionRequest(string data);

        void OnNetworkError(string data);

        void OnNetworkLatencyUpdate(int latency);

        void OnNetworkReceive(TNetPeer peer, string data);

        void OnNetworkReceiveUnconnected(string data);

        void OnPeerConnected(TNetPeer peer);

        void OnPeerDisconnected(TNetPeer peer, string data);
    }
}
