using LiteNetLib;
using System;
using TransportLayer.Interfaces;
using TransportLayer.Listeners;

namespace TransportLayer.Managers
{
    internal class LiteNetLibServerManager : IServerManager
    {
        private readonly NetManager _manager;
        private readonly LiteNetLibListener _listener;
        private readonly int _port;

        public LiteNetLibServerManager(int port, IEventListener listener, string connectionKey, int maxUsers = int.MaxValue, int timeout = 180000)
        {
            _port = port;
            if (listener == null)
                throw new Exception("Listener cannot be null.");
            _listener = new LiteNetLibListener(maxUsers, connectionKey, port, listener);
            _manager = new NetManager(_listener);
            _manager.IPv6Enabled = IPv6Mode.DualMode;
            _listener.SetManager(_manager);
            _manager.Start(port);
            _manager.DisconnectTimeout = timeout;
        }
        public void SendBroadcast(string data)
        {
            LiteNetLib.Utils.NetDataWriter writer = new LiteNetLib.Utils.NetDataWriter();
            writer.Put(data);
            foreach (NetPeer x in _manager.ConnectedPeerList)
            {
                x.Send(writer, DeliveryMethod.Unreliable);
            }
        }
        public void PollEvents()
        {
            _manager.PollEvents();
        }
        public void Stop()
        {
            _manager.Stop();
        }

    }
}
