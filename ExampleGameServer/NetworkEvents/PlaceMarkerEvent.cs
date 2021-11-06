using ExampleGameServer.Containers;
using ExampleNetModels;
using ExampleNetModels.Models;
using ServerBuilder;
using System.Threading.Tasks;
using TransportLayer;

namespace ExampleGameServer.NetworkEvents
{
    [ServerEvent(ExampleNetModels.ModelType.PlaceMarker)]
    internal class PlaceMarkerEvent : IEventFactory
    {
        public Task Construct(BaseModel baseObj, TNetPeer peer)
        {
            PlaceMarkerModel obj = baseObj.GetJsonObject<PlaceMarkerModel>();
            Modules.Game game = GameManager.Instance.GetGame(obj.GameId);

            game.PlaceMarker(obj.PlayerId, obj.Place);

            return Task.CompletedTask;
        }
    }
}
