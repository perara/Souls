using Newtonsoft.Json;
using Souls.Server.Engine;
using Souls.Server.Objects;
using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souls.Server.Game
{
    public class GameQueue
    {
        private static GameQueue instance = null;
        public LinkedList<Player> queue { get; set; }
        private GameQueue() 
        {
            queue = new LinkedList<Player>();
        }
        public bool MatchPlayers(Action<Pair<Player>> initGame)
        {
            Pair<Player> matchedPlayers = null;
			if(queue.Count() >= 2)
            {
                while (queue.Count() > 1)
                {
                    queue.OrderBy(item => item.rank);

                    Player p1 = queue.First();
                    Player p2 = queue.Skip(1).First();

                    removePlayer(p1);
                    removePlayer(p2);

                    matchedPlayers = new Pair<Player>(p1,p2);

                    // Callbacks to GameEngine's "initGame" and starts a game
                    initGame(matchedPlayers);

                }

                return true; // Matchmaking was successful
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