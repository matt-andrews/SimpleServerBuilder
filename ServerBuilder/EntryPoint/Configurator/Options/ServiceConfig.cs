using System.Collections.Generic;

namespace ServerBuilder.EntryPoint.Configurator.Options
{
    public class ServiceConfig : IConfig
    {
        public string ConfigName { get; set; }
        public string LoggerTitle { get; set; }
        public string AssemblyName { get; set; }
        public Dictionary<string, ServiceConfigEntry> Servers { get; set; }
        public Dictionary<string, ServiceConfigEntry> Clients { get; set; }

    }
}
