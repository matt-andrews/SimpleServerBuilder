using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServerBuilder.EntryPoint.Configurator;
using ServerBuilder.EntryPoint.Configurator.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ServerBuilder.EntryPoint
{
    public class EntryPointBuilder<T>
    {
        public Server Server { get; private set; }
        public string MainConfigName { get; internal set; }

        private readonly ILogger _logger;
        private readonly List<IConfig> _configs;
        private readonly List<ISetup> _setup;
        private readonly List<ISetup> _postBuildSetup;
        private readonly IDeserializer _yamlDeserializer;
        private readonly string _serviceConfigPath;

        /// <summary>
        /// Entrypoint used to deploy multiple services defined in a Yaml file.
        /// </summary>
        /// <param name="serviceConfigPath">*Required:* The path to the ServiceConfig.yaml file configured for this service.</param>
        /// <param name="logger"></param>
        public EntryPointBuilder(string serviceConfigPath, ILogger logger)
            : this(serviceConfigPath)
        {
            _logger = logger;
        }
        /// <summary>
        /// Entrypoint used to deploy multiple services defined in a Yaml file.
        /// </summary>
        /// <param name="serviceConfigPath">*Required:* The path to the ServiceConfig.yaml file configured for this service.</param>
        public EntryPointBuilder(string serviceConfigPath)
        {
            if (_logger == null)
            {
                _logger = NullLogger.Instance;
            }
            _configs = new List<IConfig>();
            _setup = new List<ISetup>();
            _postBuildSetup = new List<ISetup>();

            _yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            _serviceConfigPath = serviceConfigPath;
        }
        /// <summary>
        /// Add a configuration option and config file to the service
        /// </summary>
        /// <typeparam name="TConfig">A model of the yaml config file you're adding to this service.</typeparam>
        /// <param name="options">The operation that is invoked to process the config file</param>
        /// <param name="path">The path to the config file</param>
        /// <returns></returns>
        public EntryPointBuilder<T> AddConfigOption<TConfig>(IConfigOptions options, string path) where TConfig : IConfig
        {
            IConfig config = _configs.FirstOrDefault(f => f is TConfig);
            if (config == null)
            {
                config = _yamlDeserializer.Deserialize<TConfig>(GetFileAsString(path));
            }

            if (options != null)
            {
                options.Build(this, _logger, config);
            }
            _configs.Add(config);
            return this;
        }
        /// <summary>
        /// Add a configuration file to the service
        /// </summary>
        /// <typeparam name="TConfig">A model of the yaml config file you're adding to this service.</typeparam>
        /// <param name="path">The path to the config file</param>
        /// <returns></returns>
        public EntryPointBuilder<T> AddConfigOption<TConfig>(string path) where TConfig : IConfig
        {
            IConfig config = _configs.FirstOrDefault(f => f is TConfig);
            if (config == null)
            {
                config = _yamlDeserializer.Deserialize<TConfig>(GetFileAsString(path));
            }

            _configs.Add(config);
            return this;
        }
        /// <summary>
        /// Add configuration options and config file to the service
        /// </summary>
        /// <typeparam name="TConfig">A model of the yaml config file you're adding to this service.</typeparam>
        /// <param name="path">The path to the config file</param>
        /// <param name="options">The operations that are invoked to process the config file</param>
        /// <returns></returns>
        public EntryPointBuilder<T> AddConfigOptions<TConfig>(string path, params IConfigOptions[] options) where TConfig : IConfig
        {
            IConfig config = _configs.FirstOrDefault(f => f is TConfig);
            if (config == null)
            {
                config = _yamlDeserializer.Deserialize<TConfig>(GetFileAsString(path));
            }
            foreach (IConfigOptions option in options)
            {
                if (option != null)
                {
                    option.Build(this, _logger, config);
                }
            }
            _configs.Add(config);
            return this;
        }
        /// <summary>
        /// Add a setup option to be invoked when Build() is called
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EntryPointBuilder<T> AddSetupOption(ISetup setup)
        {
            _setup.Add(setup);
            return this;
        }
        /// <summary>
        /// Add a setup option to be invoked when Build() is called, after the service is launched, and before the first PollEvent()
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EntryPointBuilder<T> AddPostBuildSetupOption(ISetup setup)
        {
            _postBuildSetup.Add(setup);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configName">The ConfigName of the config file</param>
        /// <returns>The IConfig file with the given ConfigName</returns>
        public IConfig GetConfig(string configName)
        {
            return _configs.FirstOrDefault(f => f.ConfigName == configName);
        }

        /// <summary>
        /// Build the service
        /// </summary>
        public void Build()
        {
            foreach (ISetup setup in _setup)
            {
                setup.Invoke();
            }

            AddConfigOption<ServiceConfig>(new BuildServersOption(), _serviceConfigPath);

            foreach (ISetup setup in _postBuildSetup)
            {
                setup.Invoke();
            }

            if (Server == null)
            {
                throw new Exception("Server is null. Was BuildServerOption inturrupted?");
            }
            while (true)
            {
                Server.PollEvents();
                Thread.Sleep(15);
            }
        }
        internal void SetServers(Server servers)
        {
            Server = servers;
        }

        private string GetFileAsString(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

    }
}
