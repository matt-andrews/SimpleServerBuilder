using ExampleGameServer.Modules;
using System.Collections.Concurrent;

namespace ExampleGameServer.Containers
{
    internal class WaitingRoomManager
    {
        public static readonly WaitingRoomManager Instance = new WaitingRoomManager();
        private readonly ConcurrentBag<WaitingRoom> _list = new ConcurrentBag<WaitingRoom>();

        private WaitingRoomManager() { }
        public WaitingRoom GetRoom()
        {
            if (_list.Count > 0)
            {
                _list.TryTake(out WaitingRoom val);
                return val;
            }
            return null;
        }
        public void CreateRoom(WaitingRoom room)
        {
            _list.Add(room);
        }
    }
}
