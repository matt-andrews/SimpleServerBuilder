using ServerBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportLayer;
using TransportLayer.Interfaces;

namespace ServerBuilder.FactoryHandler
{
    public abstract class CommonEventListener<T> : IEventListener
    {
        internal readonly Dictionary<T, Factory> _cache = new Dictionary<T, Factory>();
        public abstract void OnConnectionRequest(string data);

        public abstract void OnNetworkError(string data);

        public abstract void OnNetworkLatencyUpdate(int latency);

        public abstract void OnNetworkReceiveUnconnected(string data);

        public abstract void OnPeerConnected(TNetPeer peer);

        public abstract void OnPeerDisconnected(TNetPeer peer, string data);

        public void OnNetworkReceive(TNetPeer peer, string data)
        {
            BaseModel<T> baseObj = BaseModel<T>.GetBaseObjectFromString(data);
            bool tryget = _cache.TryGetValue(baseObj.ModelType, out Factory factory);
            if (tryget)
            {
                if (factory.RequiredPorts != null &&
                    !factory.RequiredPorts.Contains(peer.Port))
                {
                    if (!factory.SilentRequiredPorts)
                    {
                        throw new Exception($"Required port was not supplied to this factory. " +
                            $"Model: {baseObj.ModelType} " +
                            $"Incoming Port: {peer.Port} " +
                            $"Required Ports: {string.Join(",", factory.RequiredPorts)}");
                    }
                }
                Task.Run(async () =>
                {
                    await factory.Interface.Construct(baseObj, peer);
                }).ConfigureAwait(false);

            }
            else
            {
                Console.WriteLine($"Cannot find event for {baseObj.ModelType}");
            }
        }
        /// <summary>
        /// Gets all attributes of type <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        internal IEnumerable<AttributeObject<TAttribute>> GetAllAttributes<TAttribute>() where TAttribute : ICommonEvent<T>
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   let attributes = t.GetCustomAttributes(typeof(TAttribute), true)
                   where attributes != null && attributes.Length > 0
                   select new AttributeObject<TAttribute> { Type = t, Attributes = attributes.Cast<TAttribute>() };

        }
        internal class AttributeObject<TAttribute> where TAttribute : ICommonEvent<T>
        {
            public Type Type { get; set; }
            public IEnumerable<TAttribute> Attributes { get; set; }
        }
    }
}
