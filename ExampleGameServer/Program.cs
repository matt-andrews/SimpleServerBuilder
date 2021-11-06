using Microsoft.Extensions.Logging;
using ExampleNetModels;
using ExampleNetModels.Config;
using ServerBuilder;
using System.IO;

namespace ExampleGameServer
{
    internal class Program
    {
        public static Server Server => _entryPoint.Server;
        public static ILogger Logger { get; private set; }

        private static ServerBuilder.EntryPoint.EntryPointBuilder<ModelType> _entryPoint;

        /// <summary>
        /// A validator used to ensure the client is using the appropriate build version to interface with the service.
        /// </summary>
        public static int RequiredClientVersion => ((CommonConfig)_entryPoint.GetConfig("Common")).RequiredClientBuild;

        private static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            Logger = loggerFactory.CreateLogger<Program>();

            _entryPoint = new ServerBuilder.EntryPoint.EntryPointBuilder<ModelType>(
                BuildRelativePath("ExampleGameServer.yaml"),
                Logger
                )
               .AddConfigOption<CommonConfig>(BuildRelativePath("Common.yaml"));

            _entryPoint.Build();
        }
        private static string BuildRelativePath(string fileName)
        {
            return Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, fileName);
        }
    }
}
