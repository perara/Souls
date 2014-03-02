using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest
{
    class QueuePool
    {



        private static QueuePool instance;
        public LinkedList<Player> queue { get; set; }
        private QueuePool() {
            queue = new LinkedList<Player>();
        }
        public static QueuePool getInstance()
        {
            if (instance == null) 
                instance = new QueuePool();
            return instance;
        }
		public bool matchPlayers(Action<List<Player>> startGame)
        {


            List<Player> matchedPlayers = null;
			if(queue.Count() >= 2)
            {
                matchedPlayers = new List<Player>();
                queue.OrderBy(item => item.rank);

                Console.WriteLine("matching players   " + queue.Count());

                while (queue.Count() > 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        matchedPlayers.Add(queue.First());
                        queue.RemoveFirst();
                    }

                    // Callbacks to GameEngine and starts a game
                    startGame(matchedPlayers);

                    // Clears the list;
                    matchedPlayers.Clear();
                }

                Console.WriteLine("matched players   " + queue.Count());
                return true; // Matchmaking was successfull
			}
            return false; // Matchmaking did not happen
		}
        public Player addPlayer(dynamic jsonObj)
        {
            Player p = new Player();
            p.id = jsonObj.id;
            p.rank = jsonObj.rank;
            p.hash = jsonObj.hash;
            this.queue.AddLast(p);
            return p;
        }

        public bool removePlayer(Player p)
        {
            return queue.Remove(p);
        }
    }
}
