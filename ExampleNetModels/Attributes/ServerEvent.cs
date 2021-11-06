using ServerBuilder.Attributes;
using System;

namespace ExampleNetModels
{
    public class ServerEvent : Attribute, IServerEvent<ModelType>
    {
        public ExampleNetModels.ModelType Type { get; }
        public int[] RequiredPorts { get; }
        public ServerEvent(ExampleNetModels.ModelType type)
        {
            Type = type;
        }
        /// <summary>
        /// Required ports are used to restrict this command to specific incoming ports.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="requiredPorts"></param>
        public ServerEvent(ExampleNetModels.ModelType type, params int[] requiredPorts)
            : this(type)
        {
            RequiredPorts = requiredPorts;
        }
    }
}
