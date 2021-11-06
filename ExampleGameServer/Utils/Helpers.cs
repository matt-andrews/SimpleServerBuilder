using ServerBuilder;
using System.Collections.Generic;
using TransportLayer;

namespace ExampleGameServer.Utils
{
    internal class Helpers
    {

    }
    internal static class BaseModelExtensions
    {
        public static T Send<T>(this T model, int destinationPeerId, TNetPeer peer) where T : BaseModel
        {
            peer.Send(destinationPeerId, model.JsonSerialize());
            return model;
        }
        public static T Send<T>(this T model, TNetPeer peer) where T : BaseModel
        {
            peer.Send(model.JsonSerialize());
            return model;
        }

        public static T SendToAll<T>(this T model, List<TNetPeer> peers) where T : BaseModel
        {
            foreach (TNetPeer x in peers)
            {
                model.Send(x);
            }
            return model;
        }
    }
}
