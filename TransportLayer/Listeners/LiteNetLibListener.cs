using LiteNetLib;
using System;
using System.Net;
using System.Net.Sockets;
using TransportLayer.Interfaces;

namespace TransportLayer.Listeners
{
    internal class LiteNetLibListener : INetEventListener
    {
        private NetManager _manager;
        private readonly int _maxUsers;
        private readonly IEventListener _listener;
        private readonly string _connectionKey;
        private readonly int _port;
        public LiteNetLibListener(int maxusers, string connectionKey, int port, IEventListener listener)
        {
            _maxUsers = maxusers;
            _listener = listener;
            _connectionKey = connectionKey;
            _port = port;
        }
        public void SetManager(NetManager manager)
        {
            _manager = manager;
        }
        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (_maxUsers == 0)
                return;

            if (_manager.ConnectedPeersCount < _maxUsers)
            {
                request.AcceptIfKey(_connectionKey);
                _listener.OnConnectionRequest("Connection Accepted");
            }
            else
            {
                _listener.OnConnectionRequest("Connection Denied: Too many peers.");
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            _listener.OnNetworkError($"Endpoint: {endPoint.Address}:{endPoint.Port} || Error: {socketError.ToString()}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            _listener.OnNetworkLatencyUpdate(latency);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            TNetPeer tpeer = new TNetPeer(peer, _port);
            string str = reader.GetString();
            _listener.OnNetworkReceive(tpeer, str);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            _listener.OnNetworkReceiveUnconnected($"Data {reader.GetString()} Message: {messageType.ToString()}");
        }

        public void OnPeerConnected(NetPeer peer)
        {
            TNetPeer tpeer = new TNetPeer(peer, _port);
            _listener.OnPeerConnected(tpeer);
        }
        public Action<NetPeer, DisconnectInfo> PeerDisconnected { get; set; }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (PeerDisconnected != null)
            {
                PeerDisconnected.Invoke(peer, disconnectInfo);
                return;
            }
            TNetPeer tpeer = new TNetPeer(peer, _port);
            _listener.OnPeerDisconnected(tpeer, $"Message: {disconnectInfo.ToString()}");
        }
    }
}
