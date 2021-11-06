using ExampleGameServer.Containers;
using ExampleNetModels;
using ExampleNetModels.Models;
using ServerBuilder;
using System.Threading.Tasks;
using TransportLayer;

namespace ExampleGameServer.NetworkEvents
{
    [ServerEvent(ExampleNetModels.ModelType.CreateGame)]
    internal class CreateGame : IEventFactory
    {
        public Task Construct(BaseModel baseObj, TNetPeer peer)
        {
            CreateGameModel obj = baseObj.GetJsonObject<CreateGameModel>();
            if (obj.IsAi)
            {
                GameManager.Instance.CreateGame(new Modules.Game(peer));
            }
            else
            {
                Modules.WaitingRoom room = WaitingRoomManager.Instance.GetRoom();
                if (room == null)
                {
                    room = new Modules.WaitingRoom(peer);
                    WaitingRoomManager.Instance.CreateRoom(room);
                }
                else
                {
                    room.SetPlayer2(peer);
                    GameManager.Instance.CreateGame(new Modules.Game(room));
                }
            }
            return Task.CompletedTask;
        }
    }
}
