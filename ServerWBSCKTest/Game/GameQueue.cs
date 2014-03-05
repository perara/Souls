using Newtonsoft.Json;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest
{
    public class GameQueue
    {
        public LinkedList<Player> queue { get; set; }
        public GameQueue() {
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
        public void addPlayer(Player player)
        {
            this.queue.AddLast(player);
        }

        public bool removePlayer(Player p)
        {
            return queue.Remove(p);
        }
    }
}