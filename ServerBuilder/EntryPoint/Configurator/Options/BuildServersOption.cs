using Microsoft.Extensions.Logging;
using ServerBuilder.FactoryHandler;
using System;
using System.Collections.Generic;
using TransportLayer.Interfaces;

namespace ServerBuilder.EntryPoint.Configurator.Options
{
    internal class BuildServersOption : IConfigOptions
    {
        public void Build<T>(EntryPointBuilder<T> entryPointBuilder, ILogger logger, IConfig configuration)
        {
            ServiceConfig config = (ServiceConfig)configuration;
            ServerBuilder server = ServerBuilder.Create();
            if (config.Servers != null)
            {
                foreach (KeyValuePair<string, ServiceConfigEntry> s in config.Servers)
                {
                    logger.LogInformation($"Starting server '{s.Key}' on port {s.Value.Port}");
                    server.WithServer(s.Key, (TransportLayer.ServerType)s.Value.TransportLayer,
                        s.Value.Port, s.Value.Key, MaxUsers(s.Value.MaxUsers), (ServerEventListener<T>)GetListener(s.Value.Listener, config), logger);
                }
            }
            if (config.Clients != null)
            {
                foreach (KeyValuePair<string, ServiceConfigEntry> c in config.Clients)
                {
                    if(c.Key == "admin_client" || c.Key == "Admin_client")
                    {
#if DEBUG
                        continue;
#endif
                    }
                    logger.LogInformation($"Connecting to server '{c.Key}' {c.Value.Ip}:{c.Value.Port}");
                    server.WithClient(c.Key, (TransportLayer.ServerType)c.Value.TransportLayer,
                        c.Value.Port, c.Value.Ip, c.Value.Key, (ClientEventListener<T>)GetListener(c.Value.Listener, config), c.Value.Reconnect);
                }
            }
            entryPointBuilder.SetServers(server.Build());
        }
        private int MaxUsers(int max)
        {
            return max == -1 ? int.MaxValue : max;
        }

        private IEventListener GetListener(string path, ServiceConfig config)
        {
            Type type = Type.GetType(path + $", {config.AssemblyName}");
            return (IEventListener)Activator.CreateInstance(type);
        }
    }
}
