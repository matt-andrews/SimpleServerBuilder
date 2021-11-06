using System.Text;
using TransportLayer.Interfaces;

namespace TransportLayer.Managers
{
    internal class TelepathyClientManager : IClientManager
    {
        private readonly Telepathy.Client _manager;
        private readonly IEventListener _listener;
        private int _id;
        private readonly int _port;
        public TelepathyClientManager(string ip, int port, string connectionKey, IEventListener listener, int timeout = 180000)
        {
            _port = port;
            _listener = listener;
            _manager = new Telepathy.Client();
            _manager.Connect(ip, port);
            _manager.SendTimeout = timeout;
        }

        public TNetPeer FirstPeer()
        {
            return new TNetPeer(_manager, _id, _port);
        }

        public void PollEvents()
        {
            while (_manager.GetNextMessage(out Telepathy.Message msg))
            {
                TNetPeer tpeer = new TNetPeer(_manager, msg.connectionId, _port);
                _id = msg.connectionId;
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        _listener.OnPeerConnected(tpeer);
                        break;
                    case Telepathy.EventType.Data:
                        UTF8Encoding utf8 = new UTF8Encoding();
                        _listener.OnNetworkReceive(tpeer, utf8.GetString(msg.data));
                        break;
                    case Telepathy.EventType.Disconnected:
                        _listener.OnPeerDisconnected(tpeer, "Disconnected");
                        break;
                }
            }
        }
        public void Disconnect()
        {
            _manager.Disconnect();
        }
    }
}
