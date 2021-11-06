using ServerBuilder.EntryPoint.Configurator;

namespace ExampleNetModels.Config
{
    public class CommonConfig : IConfig
    {
        public string ConfigName { get; set; }
        public string Build { get; set; }
        public int RequiredClientBuild { get; set; }
        public string ConfigVersion { get; set; }
    }
}
