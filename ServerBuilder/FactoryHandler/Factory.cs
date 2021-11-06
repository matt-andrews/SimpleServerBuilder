namespace ServerBuilder.FactoryHandler
{
    internal class Factory
    {
        public IEventFactory Interface { get; set; }
        /// <summary>
        /// This is a list of the required incoming transmission ports. If the peer doesn't use this port then transaction will fail. Can be null
        /// </summary>
        public int[] RequiredPorts { get; set; }
        public bool SilentRequiredPorts { get; set; }
    }
}
