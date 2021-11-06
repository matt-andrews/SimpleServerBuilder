using Microsoft.Extensions.Logging;
using Ninja.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using TransportLayer.Interfaces;

namespace TransportLayer.Managers
{
    internal class WebSocketsServer : IServerManager
    {
        private readonly ILogger _logger;
        private readonly IWebSocketServerFactory _websocketServerFactory;
        private readonly IEventListener _eventListener;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public WebSocketsServer(int port, IEventListener listener, ILogger logger, int timeout = 180000)
        {
            _logger = logger;
            _eventListener = listener;
            _websocketServerFactory = new WebSocketServerFactory();
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await StartWebServer(port);
            });
            
        }

        private async Task StartWebServer(int port)
        {
            try
            {
                using (TransportLayer.Managers.Ninja.WebServer server = new Ninja.WebServer(_websocketServerFactory, _logger, _eventListener, null))
                {
                    await server.Listen(port, _cancellationTokenSource.Token);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }

        }

        /// <summary>
        /// This does nothing in the WebSockets implementation, since polling is handled internally
        /// </summary>
        public void PollEvents()
        {

        }

        /// <summary>
        /// This does nothing in the current WebSockets implentation, perhaps in the future a record of currently connected
        /// clients will be added and a send broadcast can be used
        /// </summary>
        /// <param name="data"></param>
        public void SendBroadcast(string data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
