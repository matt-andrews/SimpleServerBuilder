using Microsoft.Extensions.Logging;
using System;
using TransportLayer.Interfaces;
using TransportLayer.Managers;

namespace TransportLayer
{
    public static class Factory
    {
        public static IServerManager BuildServer(int port, ServerType type, IEventListener listener, ILogger logger)
        {
            return BuildServer(port, type, listener, logger, "", int.MaxValue);
        }
        public static IServerManager BuildServer(int port, ServerType type, IEventListener listener, ILogger logger, string connectionKey, int maxUsers)
        {
            if (type == ServerType.LiteNetLibServer)
            {
                return new LiteNetLibServerManager(port, listener, connectionKey, maxUsers);
            }
            else if (type == ServerType.TelepathyServer)
            {
                return new TelepathyServerManager(port, listener);
            }
            else if (type == ServerType.WebSockets)
            {
                return new WebSocketsServer(port, listener, logger);
            }
            throw new Exception($"Cannot find server type {type}");
        }
        public static IClientManager BuildClient(string ip, int port, ServerType type, IEventListener listener, bool reconnect)
        {
            return BuildClient(ip, port, type, listener, "", reconnect: reconnect);
        }
        public static IClientManager BuildClient(string ip, int port, ServerType type, IEventListener listener, string connectionKey, bool reconnect)
        {
            if (type == ServerType.LiteNetLibClient)
            {
                return new LiteNetLibClientManager(ip, port, connectionKey, listener, reconnect: reconnect);
            }
            else if (type == ServerType.TelepathyClient)
            {
                return new TelepathyClientManager(ip, port, connectionKey, listener);
            }
            throw new Exception($"Cannot find client type {type}");
        }
    }
}
