using LiteNetLib;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace TransportLayer
{
    public class TNetPeer
    {
        public ServerType Type { get; }

        private readonly NetPeer _netPeer;
        private readonly Telepathy.Server _telepathyServer;
        private readonly Telepathy.Client _telepathyClient;
        private readonly WebSocket _websocket;
        public readonly int Id;
        public readonly int Port;
        public string EndPoint { get; }

        public TNetPeer(WebSocket socket)
        {
            Type = ServerType.WebSockets;
            _websocket = socket;
        }
        public TNetPeer(NetPeer peer, int port)
        {
            Type = ServerType.LiteNetLibServer;
            _netPeer = peer;
            Id = peer.Id;
            EndPoint = peer.EndPoint.Address + ":" + peer.EndPoint.Port;
            Port = port;
        }
        public TNetPeer(Telepathy.Server peer, int id, int port)
        {
            Type = ServerType.TelepathyServer;
            _telepathyServer = peer;
            Id = id;
            EndPoint = "";
            Port = port;
        }
        public TNetPeer(Telepathy.Client peer, int id, int port)
        {
            Type = ServerType.TelepathyClient;
            _telepathyClient = peer;
            Id = id;
            EndPoint = "";
            Port = port;
        }

        public void Disconnect()
        {
            if (Type == ServerType.LiteNetLibServer)
            {
                _netPeer.Disconnect();
            }
            if (Type == ServerType.TelepathyClient)
            {
                _telepathyClient.Disconnect();
            }
            if (Type == ServerType.TelepathyServer)
            {
                _telepathyServer.Disconnect(Id);
            }
            if (Type == ServerType.WebSockets)
            {
                _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested disconnect", CancellationToken.None);
            }
        }
        public void Send(string data)
        {
            Send(Id, data, DeliveryMethod.ReliableUnordered);
        }
        public void Send(string data, LiteNetLib.DeliveryMethod deliveryMethod)
        {
            Send(Id, data, deliveryMethod);
        }
        public void Send(int destinationPeerId, string data, LiteNetLib.DeliveryMethod deliveryMethod = DeliveryMethod.ReliableUnordered)
        {
            if (Type == ServerType.LiteNetLibServer)
            {
                LiteNetLib.Utils.NetDataWriter writer = new LiteNetLib.Utils.NetDataWriter();
                writer.Put(data);
                if (_netPeer == null)
                    return;
                _netPeer.Send(writer, deliveryMethod);
            }
            else if (Type == ServerType.TelepathyClient)
            {
                if (_telepathyClient == null)
                    return;
                _telepathyClient.Send(Utf8Encode(data));
            }
            else if (Type == ServerType.TelepathyServer)
            {
                if (_telepathyServer == null)
                    return;
                _telepathyServer.Send(destinationPeerId, Utf8Encode(data));
            }
            else if (Type == ServerType.WebSockets)
            {
                byte[] array = Encoding.UTF8.GetBytes(data);
                ArraySegment<byte> buffer = new ArraySegment<byte>(array);
                if (_websocket == null)
                    return;
                _websocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        private byte[] Utf8Encode(string data)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encode = utf8.GetBytes(data);
            return encode;
        }
    }
}
