using Microsoft.Extensions.Logging;
using ServerBuilder.FactoryHandler;
using System;
using System.Collections.Generic;
using TransportLayer.Interfaces;

namespace ServerBuilder
{
    public class Server
    {
        private readonly Dictionary<string, ICommon> _servers;
        internal Server(Dictionary<string, ICommon> list)
        {
            _servers = list;
        }
        public IClientManager GetClientManager(string id)
        {
            if (_servers[id] is IClientManager client)
            {
                return client;
            }
            throw new Exception("Given id is not a valid id or is not a client");
        }
        public void PollEvents()
        {
            foreach (ICommon mgr in _servers.Values)
            {
                mgr.PollEvents();
            }
        }
        public IServerManager GetServerManager(string id)
        {
            if (_servers[id] is IServerManager server)
            {
                return server;
            }
            throw new Exception("Given id is not a valid id or is not a server");
        }
        public IServerManager[] GetServers()
        {
            List<IServerManager> temp = new List<IServerManager>();
            foreach (ICommon x in _servers.Values)
            {
                if (x is IServerManager server)
                {
                    temp.Add(server);
                }
            }
            return temp.ToArray();
        }
        public IClientManager[] GetClients()
        {
            List<IClientManager> temp = new List<IClientManager>();
            foreach (ICommon x in _servers.Values)
            {
                if (x is IClientManager client)
                {
                    temp.Add(client);
                }
            }
            return temp.ToArray();
        }
        public void Stop()
        {
            foreach (ICommon mgr in _servers.Values)
            {
                if (mgr is IServerManager server)
                {
                    server.Stop();
                }
            }
        }
        public void Disconnect()
        {
            foreach (ICommon mgr in _servers.Values)
            {
                if (mgr is IClientManager client)
                {
                    client.Disconnect();
                }
            }
        }

    }
    public class ServerBuilder
    {
        private readonly Dictionary<string, ICommon> _list = new Dictionary<string, ICommon>();
        public static ServerBuilder Create()
        {
            return new ServerBuilder();
        }
        public void Add(string id, ICommon mgr)
        {
            _list.Add(id, mgr);
        }
        public Server Build()
        {
            return new Server(_list);
        }
    }
    public static class ServerExtensions
    {
        public static ServerBuilder WithServer<T>(this ServerBuilder builder, string id, TransportLayer.ServerType type, int port, ServerEventListener<T> listener, ILogger logger)
        {
            builder.Add(id, TransportLayer.Factory.BuildServer(port, type, listener, logger));
            return builder;
        }
        public static ServerBuilder WithServer<T>(this ServerBuilder builder, string id, TransportLayer.ServerType type, int port, string key, int maxUsers, ServerEventListener<T> listener, ILogger logger)
        {
            builder.Add(id, TransportLayer.Factory.BuildServer(port, type, listener, logger, key, maxUsers));
            return builder;
        }
        public static ServerBuilder WithClient<T>(this ServerBuilder builder, string id, TransportLayer.ServerType type, int port, string ip, ClientEventListener<T> listener)
        {
            builder.Add(id, TransportLayer.Factory.BuildClient(ip, port, type, listener, reconnect:false));
            return builder;
        }
        public static ServerBuilder WithClient<T>(this ServerBuilder builder, string id, TransportLayer.ServerType type, int port, string ip, string key, ClientEventListener<T> listener, bool reconnect = false)
        {
            builder.Add(id, TransportLayer.Factory.BuildClient(ip, port, type, listener, key, reconnect:reconnect));
            return builder;
        }
    }
}
