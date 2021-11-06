using Microsoft.Extensions.Logging;
using ExampleNetModels;
using ExampleNetModels.Models;
using ServerBuilder;
using System.Threading.Tasks;
using TransportLayer;

namespace ExampleGameServer.NetworkEvents
{
    [ServerEvent(ExampleNetModels.ModelType.ConnectionState)]
    internal class ConnectionState : IEventFactory
    {
        public Task Construct(BaseModel baseObj, TNetPeer peer)
        {
            ConnectionStateModel obj = baseObj.GetJsonObject<ConnectionStateModel>();
            if (obj.IsConnected)
            {
                Program.Logger.LogInformation("User Connected: {0}", peer.Id);
            }
            else
            {
                peer.Disconnect();
            }
            return Task.CompletedTask;
        }
    }
}
