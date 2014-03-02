using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWBSCKTest
{
    class GameEngine
    {
        public GameEngine()
        {

        }

        public void pollQueue()
        {
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    bool matchMaked = QueuePool.getInstance().matchPlayers(initGame);
                    Thread.Sleep(15000);
                    Console.WriteLine(DateTime.Now - Process.GetCurrentProcess().StartTime + ": Polling Queue - " + ((matchMaked) ? "Matchmaking this round" :  "No matchmaking this round"));
                }
            });

            pollThread.Start();
        }

        public void initGame(List<Player> players)
        {
            foreach(Player p in players)
            {
                Console.WriteLine(p.id);
  
            }
            Console.WriteLine("--");
        }
    }
}
