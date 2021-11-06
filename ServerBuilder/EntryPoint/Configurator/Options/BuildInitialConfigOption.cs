using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBuilder.EntryPoint.Configurator.Options
{
    public class BuildInitialConfigOption : IConfigOptions
    {
        public void Build<T>(EntryPointBuilder<T> entryPointBuilder, ILogger logger, IConfig configuration)
        {
            ServiceConfig config = (ServiceConfig)configuration;
            entryPointBuilder.MainConfigName = config.ConfigName;
        }
    }
}
