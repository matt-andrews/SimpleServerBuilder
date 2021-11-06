using ExampleGameServer.Containers;
using ExampleGameServer.Utils;
using ExampleNetModels.Models;
using ServerBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using TransportLayer;

namespace ExampleGameServer.Modules
{
    internal class Game
    {
        public int GameId { get; }
        public Team CurrentTeamTurn { get; private set; }
        public int TurnNumber { get; private set; }
        public string[] Squares { get; } = new string[9];
        public Player[] Players { get; }
        public System.Timers.Timer Timer { get; private set; }
        public int TimeoutInterval => 30 * 1000;
        public bool IsAi { get; }

        private Game()
        {
            GameId = (int)(DateTime.Now.ToFileTimeUtc() / 2);
        }
        /// <summary>
        /// Create a game for multiplayer
        /// </summary>
        /// <param name="room"></param>
        public Game(WaitingRoom room)
            : this()
        {
            if (room.Player1 == null || room.Player2 == null)
            {
                string player = room.Player1 == null ? "1" : "2";
                throw new Exception($"Player {player} was null!");
            }
            //create players collection and send game created data to each client
            Players = new Player[]
            {
                new Player(room.Player1, Team.X),
                new Player(room.Player2, Team.O)
            };
            CreateGame();
        }
        /// <summary>
        /// Constructor used for AI battle
        /// </summary>
        /// <param name="player1"></param>
        public Game(TNetPeer player1)
            : this()
        {
            if (player1 == null)
            {
                throw new Exception("Player was null!");
            }
            IsAi = true;
            Players = new Player[]
            {
                new Player(player1, Team.X),
                new Player(Team.O, true)
            };
            CreateGame();
        }
        /// <summary>
        /// Sends the CreateGameModel to all players with the game specifics
        /// </summary>
        private void CreateGame()
        {
            foreach (Player player in Players)
            {
                if (player.Peer != null)
                {
                    new GameCreatedModel()
                    {
                        GameId = GameId,
                        PlayerTeam = (int)player.PlayerTeam,
                        Turn = (int)CurrentTeamTurn,
                        PlayerId = player.PlayerId
                    }.Send(player.Peer);
                }
            }
        }

        public void PlaceMarker(string playerid, int marker)
        {
            Player player = Players.FirstOrDefault(f => f.PlayerId == playerid);
            if (player == null)
            {
                //error?
                return;
            }
            if (Squares[marker] != null)
            {
                //error?
                return;
            }
            if (CurrentTeamTurn == player.PlayerTeam)
            {
                Squares[marker] = CurrentTeamTurn == Team.X ? "X" : "O";
                if (NewTurn())
                {
                    return;
                }
                UpdateGameState();
            }
            else
            {
                //error?
                return;
            }
            //do ai battle if applicable
            AiTurn();
        }
        /// <summary>
        /// Does the ai turn if this is an ai battle and it is the ai's turn
        /// </summary>
        private void AiTurn()
        {
            if (IsAi && CurrentTeamTurn == Team.O)
            {
                //get random marker
                List<int> pool = new List<int>();
                for (int i = 0; i < Squares.Length; i++)
                {
                    string square = Squares[i];
                    if (square == null)
                    {
                        pool.Add(i);
                    }
                }
                if (pool.Count == 0)
                {
                    return;
                }
                Random random = new Random();
                int rng = random.Next(0, pool.Count);
                PlaceMarker(Players[1].PlayerId, pool.ElementAt(rng));
            }
        }
        /// <summary>
        /// Returns true if win condition is true
        /// </summary>
        /// <returns></returns>
        private bool NewTurn()
        {
            CurrentTeamTurn = CurrentTeamTurn == Team.X ? Team.O : Team.X;
            TurnNumber++;
            RestartTimer();
            return WinCondition();
        }
        /// <summary>
        /// Restarts the turn timeout timer by disposing of the current, and creating it again. Probably not the most efficient implementation,
        /// but does the job for this example.
        /// </summary>
        private void RestartTimer()
        {
            if (Timer != null)
            {
                Timer.Dispose();
            }
            Timer = new System.Timers.Timer
            {
                Interval = TimeoutInterval
            };
            Timer.Elapsed += (s, e) =>
            {
                if (NewTurn())
                {
                    return;
                }
                UpdateGameState();
                AiTurn();
            };

            Timer.Start();
        }

        private void UpdateGameState()
        {
            UpdateGameStateModel model = new UpdateGameStateModel()
            {
                Complete = false,
                Squares = Squares,
                Turn = (int)CurrentTeamTurn
            };
            SendToAll(model);
        }

        /// <summary>
        /// Sends to all active players
        /// </summary>
        /// <param name="model"></param>
        private void SendToAll(BaseModel model)
        {
            foreach (Player player in Players)
            {
                if (player.Peer != null)
                {
                    model.Send(player.Peer);
                }
            }
        }
        /// <summary>
        /// Evaluates a win condition by going through the array of acceptable win conditions and looks for 3 of the same marker
        /// returns true if game is over, regardless of the win type. Calls GameEnd() when game is over.
        /// </summary>
        /// <returns></returns>
        private bool WinCondition()
        {
            int[][] winConditions = new int[][]
            {
                new int[]{ 0, 1, 2 },
                new int[]{ 3, 4, 5 },
                new int[]{ 6, 7, 8 },
                new int[]{ 0, 3, 6 },
                new int[]{ 1, 4, 7 },
                new int[]{ 2, 5, 8 },
                new int[]{ 0, 4, 8 },
                new int[]{ 2, 4, 6 }
            };
            bool winCondition = false;
            Team winner = Team.Draw;
            foreach (int[] condition in winConditions)
            {
                int x = 0;
                int o = 0;
                foreach (int item in condition)
                {
                    if (Squares[item] == null)
                    {
                        continue;
                    }
                    if (Squares[item] == "X")
                    {
                        x++;
                    }
                    if (Squares[item] == "O")
                    {
                        o++;
                    }
                }
                if (x == 3)
                {
                    winner = Team.X;
                    winCondition = true;
                    break;
                }
                if (o == 3)
                {
                    winner = Team.O;
                    winCondition = true;
                    break;
                }
            }
            if (winCondition)
            {
                GameEnd(winner);
                return true;
            }
            else
            {
                //this condition signifies a draw
                //either all the squares are taken, or turn number exceeds 10 turns
                if (!Squares.Contains(null) || TurnNumber > 10)
                {
                    GameEnd(Team.Draw);
                    return true;
                }
            }
            return false;
        }

        private void GameEnd(Team winner)
        {
            Timer.Dispose();
            UpdateGameStateModel model = new UpdateGameStateModel()
            {
                Complete = true,
                Winner = (int)winner,
                Squares = Squares
            };
            SendToAll(model);
            GameManager.Instance.RemoveGame(this);
        }
    }
}
