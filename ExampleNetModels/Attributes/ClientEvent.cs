using ServerBuilder.Attributes;
using System;

namespace ExampleNetModels
{
    public class ClientEvent : Attribute, IClientEvent<ModelType>
    {
        public ExampleNetModels.ModelType Type { get; }
        public int[] RequiredPorts { get; }
        public bool SilentRequiredPorts { get; }
        public ClientEvent(ExampleNetModels.ModelType type)
        {
            Type = type;
        }
        /// <summary>
        /// Required ports are used to restrict this command to specific incoming ports. An exception will be thrown unless you set `silentREquiredPort` to true.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="requiredPorts"></param>
        public ClientEvent(ExampleNetModels.ModelType type, params int[] requiredPorts)
            : this(type)
        {
            RequiredPorts = requiredPorts;
        }
        /// <summary>
        /// With silentRequiredPort true, in the event port requirement is not met an exception will not be thrown and the command will simply be ignored.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="silentRequiredPort"></param>
        /// <param name="requiredPorts"></param>
        public ClientEvent(ExampleNetModels.ModelType type, bool silentRequiredPort, params int[] requiredPorts)
            : this(type, requiredPorts)
        {
            SilentRequiredPorts = silentRequiredPort;
        }
    }
}
