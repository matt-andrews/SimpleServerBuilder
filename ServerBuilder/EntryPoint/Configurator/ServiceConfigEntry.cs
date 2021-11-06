namespace ServerBuilder.EntryPoint
{
    public class ServiceConfigEntry
    {
        public string Key { get; set; }
        public int MaxUsers { get; set; }
        public int TransportLayer { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }
        public string Listener { get; set; }
        public bool Reconnect { get; set; }
    }
}
