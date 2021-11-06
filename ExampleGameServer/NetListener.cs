using ExampleGameServer.Utils;
using Microsoft.Extensions.Logging;
using ExampleNetModels;
using ExampleNetModels.Models;
using ServerBuilder.FactoryHandler;
using TransportLayer;

namespace ExampleGameServer
{
    internal class NetListener : ServerEventListener<ModelType>
    {
        public override void OnConnectionRequest(string data)
        {
        }

        public override void OnNetworkError(string data)
        {
        }

        public override void OnNetworkLatencyUpdate(int latency)
        {
        }


        public override void OnNetworkReceiveUnconnected(string data)
        {
        }

        public override void OnPeerConnected(TNetPeer peer)
        {
            Program.Logger.LogInformation("User Connected: {0}", peer.Id);
            new ConnectionStateModel() { IsConnected = true }.Send(peer);
        }

        public override void OnPeerDisconnected(TNetPeer peer, string data)
        {
            Program.Logger.LogInformation("User Disconnected: {0}", peer.Id);
        }
    }
}
