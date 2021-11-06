using ExampleGameServer.Modules;
using System.Collections.Concurrent;

namespace ExampleGameServer.Containers
{
    internal class GameManager
    {
        public static readonly GameManager Instance = new GameManager();
        private readonly ConcurrentDictionary<long, Game> _list = new ConcurrentDictionary<long, Game>();

        private GameManager() { }
        public Game GetGame(long gameId)
        {
            if (_list.TryGetValue(gameId, out Game val))
            {
                return val;
            }
            return null;
        }
        public void CreateGame(Game game)
        {
            _list.TryAdd(game.GameId, game);
        }
        public void RemoveGame(Game game)
        {
            _list.TryRemove(game.GameId, out Game val);
        }
    }
}
