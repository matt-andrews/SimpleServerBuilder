using System;
using TransportLayer;

namespace ExampleGameServer.Modules
{
    internal class Player
    {
        public TNetPeer Peer { get; }
        public string PlayerId { get; }
        public Team PlayerTeam { get; }
        public bool IsAi { get; }
        private Player(Team team)
        {
            PlayerTeam = team;
            PlayerId = Guid.NewGuid().ToString();
        }
        public Player(TNetPeer peer, Team team)
            : this(team)
        {
            Peer = peer;
        }
        public Player(Team team, bool isAi)
            : this(team)
        {
            PlayerTeam = team;
        }
    }
}
