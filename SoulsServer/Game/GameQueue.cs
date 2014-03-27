using Newtonsoft.Json;
using SoulsServer.Engine;
using SoulsServer.Objects;
using SoulsServer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsServer.Game
{
    public class GameQueue
    {
        private static GameQueue instance = null;
        public LinkedList<Player> queue { get; set; }
        private GameQueue() 
        {
            queue = new LinkedList<Player>();
        }
        public bool matchPlayers(Action<Pair<Player>> initGame)
        {
            Pair<Player> matchedPlayers = null;
			if(queue.Count() >= 2)
            {
                while (queue.Count() > 1)
                {
                    queue.OrderBy(item => item.rank);
                    matchedPlayers = new Pair<Player>(queue.First(), queue.Skip(1).First());
                    queue.RemoveFirst();
                    queue.RemoveFirst();

                    // Callbacks to GameEngine's "initGame" and starts a game
                    initGame(matchedPlayers);

                }

                return true; // Matchmaking was successfull
			}
            return false; // Matchmaking did not happen
		}
        public void AddPlayer(Player player)
        {
            player.inQueue = true;
            this.queue.AddLast(player);
        }

        public bool removePlayer(Player player)
        {
            player.inQueue = false;
            return queue.Remove(player);
        }

        public static GameQueue GetInstance()
        {
            if (GameQueue.instance == null) GameQueue.instance = new GameQueue();
            return instance;
            
        }
    }
}