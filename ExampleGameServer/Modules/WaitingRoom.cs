using System;
using TransportLayer;

namespace ExampleGameServer.Modules
{
    internal class WaitingRoom
    {
        public long RoomId { get; private set; }
        public TNetPeer Player1 { get; }
        public TNetPeer Player2 { get; private set; }
        public WaitingRoom(TNetPeer player1)
        {
            RoomId = DateTime.Now.ToFileTimeUtc();
            Player1 = player1;
        }
        public void SetPlayer2(TNetPeer player2)
        {
            Player2 = player2;
        }
    }
}
