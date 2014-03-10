using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json;
using ServerWBSCKTest.Game;
using Alchemy.Classes;

namespace ServerWBSCKTest
{
    class Runner
    {
        static void Main(string[] args)
        {
            // Initialize GameEngine
            GameEngine engine = new GameEngine();

            // Initialize the server
            Server srv = new Server(engine);
         

            // Create a game
            GameRoom game = new GameRoom();

            // Generate game data
            GameData data = new GameData(game);

            var r = new Response { Type = ResponseType.Error, Data = new { Message = errorMessage } };
            Server.SendTo(Server.OnlinePlayers[game.p1], new Response {  })



            //engine.pollQueue();
            /*
            game.p1.cards.Concat(game.getRandomCards(2));
            //game.p1.cards.AddFirst(game.getRandomCard());

            foreach (Card card in game.p1.cards)
            {
                Console.WriteLine(card);
            }

            SQLCommands sqlc = new SQLCommands();
            sqlc.connect();
            */




            
        }
    }
}
