using ServerBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerBuilder.FactoryHandler
{
    public abstract class ClientEventListener<T> : CommonEventListener<T>
    {
        public ClientEventListener()
        {
            IEnumerable<AttributeObject<IClientEvent<T>>> types = GetAllAttributes<IClientEvent<T>>();
            foreach (AttributeObject<IClientEvent<T>> x in types)
            {
                IEventFactory factory = (IEventFactory)Activator.CreateInstance(x.Type);
                IClientEvent<T> attribute = x.Attributes.First();
                _cache.Add(attribute.Type, new Factory()
                {
                    Interface = factory,
                    RequiredPorts = attribute.RequiredPorts,
                    SilentRequiredPorts = attribute.SilentRequiredPorts
                });
            }
        }
    }
}
