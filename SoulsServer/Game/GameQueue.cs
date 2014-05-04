using Newtonsoft.Json;
using Souls.Server.Engine;
using Souls.Server.Objects;
using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using SoulsServer.Objects;
using System.Threading;

namespace Souls.Server.Game
{
    public class GameQueue
    {
        private static GameQueue instance = null;
        public LinkedList<Player> normalQueue { get; set; }
        public LinkedList<Player> practiceQueue { get; set; }
        private GameQueue()
        {
            normalQueue = new LinkedList<Player>();
            practiceQueue = new LinkedList<Player>();
        }
        public bool MatchPlayersNormal(Action<Pair<Player>> initGame)
        {

            // Remove disconnected players
            Player discP = normalQueue.Where(x => x.gameContext.State == WebSocketState.Closed).FirstOrDefault();
            if (discP != null) normalQueue.Remove(discP);


            Pair<Player> matchedPlayers = null;
            if (normalQueue.Count() >= 2)
            {
                while (normalQueue.Count() > 1)
                {
                    normalQueue.OrderBy(item => item.rank);

                    Player p1 = normalQueue.First();
                    Player p2 = normalQueue.Skip(1).First();

                    RemovePlayerNormal(p1);
                    RemovePlayerNormal(p2);

                    matchedPlayers = new Pair<Player>(p1, p2);

                    // Callbacks to GameEngine's "initGame" and starts a game
                    initGame(matchedPlayers);

                }

                return true; // Matchmaking was successful
            }
            return false; // Matchmaking did not happen
        }

        public bool MatchPlayersPractice(Action<Pair<Player>> initGame)
        {
            // If anyone queued
            if (practiceQueue.Count >= 1)
            {
                AI p = new AI();
                p.Connect();

                Player p1 = practiceQueue.First();
                RemovePlayerPractice(p1);

                Player p2 = null;
                while (p2 == null)
                {

                    Player bot = practiceQueue.Where(x => x.name == "BOT").FirstOrDefault();
                    if (bot != null)
                    {
                        Random r = new Random(System.DateTime.Now.Millisecond);
                        string botName = GameEngine.botNames.ElementAt(r.Next(GameEngine.botNames.Count));
                        botName = "[BOT] " + botName;
                       
                        p2 = bot;
                        p2.name = botName;
                        p2.chatContext = p2.gameContext; // Same context for both! 
                        p2.chPlayer = new ChatPlayer(botName);
                        p2.chPlayer.chatContext = p2.chatContext;
                        
                        RemovePlayerPractice(bot);
                    }

                    Thread.Sleep(500);
                }

                Pair<Player> matchedPlayers = new Pair<Player>(p1, p2);
                

                // Callbacks to GameEngine's "initGame" and starts a game
                initGame(matchedPlayers);
            }


            return true;
        }


        public void AddPlayerNormal(Player player)
        {
            player.inQueue = true;
            this.normalQueue.AddLast(player);
        }

        public bool RemovePlayerNormal(Player player)
        {
            player.inQueue = false;
            return normalQueue.Remove(player);
        }

        public void AddPlayerPractice(Player player)
        {
            player.inQueue = true;
            this.practiceQueue.AddLast(player);
        }

        public bool RemovePlayerPractice(Player player)
        {
            player.inQueue = false;
            return practiceQueue.Remove(player);
        }

        public static GameQueue GetInstance()
        {
            if (GameQueue.instance == null) GameQueue.instance = new GameQueue();
            return instance;

        }
    }
}