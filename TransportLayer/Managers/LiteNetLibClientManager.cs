using LiteNetLib;
using System;
using TransportLayer.Interfaces;
using TransportLayer.Listeners;

namespace TransportLayer.Managers
{
    internal class LiteNetLibClientManager : IClientManager
    {
        private NetManager _manager;
        private readonly LiteNetLibListener _listener;
        private readonly int _port;
        public LiteNetLibClientManager(string ip, int port, string connectionKey, IEventListener listener, int timeout = 180000, bool reconnect = false)
        {
            _port = port;
            _listener = new LiteNetLibListener(0, connectionKey, port, listener);
            if (reconnect)
            {
                _listener.PeerDisconnected = (p, e) =>
                {
                    Console.WriteLine($"Client disconnected, trying to reconnect. {ip}:{port}");
                    _manager = new NetManager(_listener);
                    _listener.SetManager(_manager);
                    _manager.Start();
                    _manager.Connect(ip, port, connectionKey);
                    _manager.DisconnectTimeout = timeout;
                };
            }
            _manager = new NetManager(_listener);
            _listener.SetManager(_manager);
            _manager.Start();
            _manager.Connect(ip, port, connectionKey);
            _manager.DisconnectTimeout = timeout;
        }
        public void PollEvents()
        {
            _manager.PollEvents();
        }
        public TNetPeer FirstPeer()
        {
            return new TNetPeer(_manager.FirstPeer, _port);
        }

        public void Disconnect()
        {
            _manager.DisconnectPeer(_manager.FirstPeer);
        }
    }
}
