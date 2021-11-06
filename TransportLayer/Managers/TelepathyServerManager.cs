using System.Text;
using TransportLayer.Interfaces;

namespace TransportLayer.Managers
{
    internal class TelepathyServerManager : IServerManager
    {
        private readonly Telepathy.Server _manager;
        private readonly IEventListener _listener;
        private readonly int _port;

        public TelepathyServerManager(int port, IEventListener listener, int timeout = 180000)
        {
            _listener = listener;
            _port = port;
            _manager = new Telepathy.Server();
            _manager.Start(port);
            _manager.SendTimeout = timeout;
        }
        public void SendBroadcast(string data)
        {

        }
        public void PollEvents()
        {
            Telepathy.Message msg;
            while (_manager.GetNextMessage(out msg))
            {
                TNetPeer tpeer = new TNetPeer(_manager, msg.connectionId, _port);
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
        public void Stop()
        {
            _manager.Stop();
        }
    }
}
