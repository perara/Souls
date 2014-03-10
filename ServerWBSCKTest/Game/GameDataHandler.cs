using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Engine
{
    public class GameDataHandler
    {
        GameEngine gameEngine = new GameEngine();
        public GameDataHandler(GameEngine engine)
        {
            gameEngine = engine;
        }
        public void trafficHandler(UserContext context, dynamic game)
        {

            // NON GAME LOGIC REQUESTS
            switch ((GameType)game.Type)
            {

                // GAME LOGIC REQUESTS
                case GameType.QUEUE:
                    gameEngine.QueuePlayer(SocketServer.OnlinePlayers[context]);
                    break;

                case GameType.ATTACK:
                    gameEngine.RequestCardAttack(game.Payload);
                    Console.WriteLine(game.Payload);
                    break;

                case GameType.USECARD:
                    gameEngine.RequestUseCard(game.Payload);
                    Console.WriteLine(game.Payload);
                    break;

                case GameType.NEXTROUND:
                    gameEngine.RequestNextRound(game.Payload);
                    break;

                

            }
        }

        public enum ResponseType
        {
            // Queue
            QUEUED_OK = 3,
            QUEUED_ERROR = 4,

            // Game
            GAME_UPDATE = 2,
            GAME_OOM = 5,

            // General
            ERROR = 25,
            CHAT_MESSAGE = 26,

            // CHEAT DETECTUS
            CHEAT = 1337,

            // Server only handle
            CONNECTION = 200,
            DISCONNECT = 201,
        }


        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        public enum GameType
        {
            // Game Stage 
            QUEUE = 3,
            ATTACK = 4,
            USECARD = 5,
            NEXTROUND = 6,
        }
    }
}
