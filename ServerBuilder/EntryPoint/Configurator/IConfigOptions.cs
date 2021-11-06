using Microsoft.Extensions.Logging;

namespace ServerBuilder.EntryPoint.Configurator
{
    public interface IConfigOptions
    {
        void Build<T>(EntryPointBuilder<T> entryPointBuilder, ILogger logger, IConfig configuration);
    }
}
