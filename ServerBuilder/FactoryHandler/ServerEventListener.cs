using ServerBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerBuilder.FactoryHandler
{
    public abstract class ServerEventListener<T> : CommonEventListener<T>
    {
        public ServerEventListener()
        {
            IEnumerable<AttributeObject<IServerEvent<T>>> types = GetAllAttributes<IServerEvent<T>>();
            foreach (AttributeObject<IServerEvent<T>> x in types)
            {
                IEventFactory factory = (IEventFactory)Activator.CreateInstance(x.Type);
                IServerEvent<T> attribute = x.Attributes.First();
                _cache.Add(attribute.Type, new Factory()
                {
                    Interface = factory,
                    RequiredPorts = attribute.RequiredPorts
                });
            }
        }
    }
}
