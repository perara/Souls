using Souls.Server.Game;
using Souls.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using Souls.Server.Tools;
using Souls.Server.Objects;

namespace SoulsServer.Network
{
    public class GameService : Service
    {

        /// <summary>
        /// This is the response type which the servers sends back to the client
        /// GENERAL: 0-99
        /// QUEUE: 100 - 199
        /// GAME: 200-299
        /// </summary>
        public enum GameResponseType
        {
            // Queue
            QUEUE_OK = 100,
            QUEUE_ALREADY_IN = 101,
            QUEUE_UNQUEUE = 102,
            QUEUE_ERROR = 103,

            // Game
            GAME_UPDATE = 201,
            GAME_AUTH_FAIL = 204,
            GAME_CREATE = 206,

            // Player
            GAME_USECARD_OK = 207,
            GAME_USECARD_OOM = 208,
            GAME_USECARD_OCCUPIED = 213,
            GAME_USECARD_SPELL = 224,
            GAME_RELEASE = 212,
            GAME_NEWCARD = 222,
            GAME_NEXT_TURN = 226,
            GAME_NOT_YOUR_TURN = 202,
            GAME_NEXT_ROUND_FAIL = 203,
            GAME_ATTACK = 218,
            // GAME_CARD_DIE = 219,
            // GAME_HERO_ATTACK = 220,
            // GAME_HERO_DIE = 221, // Game lost

            // Opponent
            GAME_OPPONENT_NOEXIST = 205,
            GAME_OPPONENT_USECARD_OK = 211,
            GAME_OPPONENT_USECARD_SPELL = 225,
            GAME_OPPONENT_NEWCARD = 223,
            GAME_OPPONENT_MOVE = 209,
            GAME_OPPONENT_RELEASE = 210,
            GAME_OPPONENT_ATTACK = 214,
            // GAME_OPPONENT_CARD_DIE = 215,
            // GAME_OPPONENT_HERO_ATTACK = 216,
            // GAME_OPPONENT_HERO_DIE = 217, // Game Won

            GAME_RECOVER = 220, // When the client disconnected and needs a recover update.

            GAME_VICTORY = 230,
            GAME_DEFEAT = 231
        }


        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        private enum GameType
        {
            // Queue 
            QUEUE = 100,

            // Game
            ATTACK = 200, // subtypes: 0 = Card on Card | 1 = Card on hero | 2 = Hero on Card
            USECARD = 201,

            NEXT_TURN = 226,
            MOVE_CARD = 203,
            RELEASE_CARD = 204

        }

        public GameEngine engine;

        public GameService(GameEngine engine)
        {
            this.engine = engine;
            this.logType = Logging.Type.GAME;
        }

        public override void Process()
        {

            switch ((GameType)type)
            {
                // GAME LOGIC REQUESTS
                case GameType.QUEUE:
                    engine.Request_QueuePlayer(Clients.GetInstance().gameList[this]);
                    break;
                case GameType.ATTACK:
                    engine.Request_Attack(Clients.GetInstance().gameList[this]);
                    break;
                case GameType.USECARD:
                    engine.Request_UseCard(Clients.GetInstance().gameList[this]);
                    break;
                case GameType.NEXT_TURN:
                    engine.Request_NextTurn(Clients.GetInstance().gameList[this]);
                    break;
                case GameType.MOVE_CARD:
                    engine.Request_MoveCard(Clients.GetInstance().gameList[this]);
                    break;
                case GameType.RELEASE_CARD:
                    engine.Request_OpponentReleaseCard(Clients.GetInstance().gameList[this]);
                    break;
            }

            switch ((SERVICE)type)
            {
                case SERVICE.LOGIN:
                    this.Login();
                    break;
                case SERVICE.LOGOUT:
                    this.Logout();
                    break;
                case SERVICE.HEARTBEAT:
                    this.HeartBeat();
                    break;

            }
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the gameList of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        public override void Login()
        {
            string hash = this.payload["hash"].ToString();

            //////////////////////////////////////////////////////////////////////////
            // If no hash exists
            //////////////////////////////////////////////////////////////////////////
            if (hash == "")
            {
                this.SendTo(
                    new Response(Service.SERVICE_RESPONSE.LOGIN_NO_HASH, "No Hash")
                );

                this.Context.WebSocket.Close(WebSocketSharp.CloseStatusCode.Away, "No Hash");
                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // Check if the client is already logged in (Recover from disconnect)
            //////////////////////////////////////////////////////////////////////////
            Client searchClient = Clients.GetInstance().gameList.Where(x => x.Value.UpdateHash() == hash).FirstOrDefault().Key;

            if(searchClient != null)
            {
                this.SwapClient(searchClient);
                SendTo(new Response(SERVICE_RESPONSE.LOGIN_OK, "Logged in as " + Clients.GetInstance().gameList[this].name));

                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // First time Login (Normal login)
            //////////////////////////////////////////////////////////////////////////

            Player player = new Player();
            player.hash = hash;
            player.gameContext = this;
            bool success = player.FetchPlayerInfo();
            
            Clients.GetInstance().gameList.TryAdd(this, player);

            if (success)
            {
                SendTo(new Response(SERVICE_RESPONSE.LOGIN_OK, "Logged in as " + Clients.GetInstance().gameList[this].name));
                Logging.Write(Logging.Type.GENERAL, "Client: " + Context.UserEndPoint + " authenticated.");
                Logging.Write(Logging.Type.GENERAL, "Online players: " + Clients.GetInstance().gameList.Count());
            }

        }

        public override void Logout()
        {
            bool success = false;
            try
            {

                GameQueue.GetInstance().removePlayer(Clients.GetInstance().gameList[this]);

                Player trash;
                success = Clients.GetInstance().gameList.TryRemove(this, out trash);

                if (success)
                {
                    Response response = new Response(SERVICE_RESPONSE.DISCONNECTED, "You are now logged out!");
                    SendTo(response);
                    Logging.Write(Logging.Type.GENERAL, "Client authenticated: " + Context.UserEndPoint);
                    Logging.Write(Logging.Type.GENERAL, "Online players: " + Clients.GetInstance().gameList.Count());
                }
            }
            catch (Exception exception) // Bad JSON! For shame.
            {
                var response = new Response(SERVICE_RESPONSE.ERROR, exception.Message);
                SendTo(response);
            }
        }


        /// <summary>
        /// Swaps out a client with another in the OnlinePlayers gameList
        /// </summary>
        public void SwapClient(Client with)
        {

            Player outP;

            // Gets the Player object
            Clients.GetInstance().gameList.TryRemove(with, out outP);

            Clients.GetInstance().gameList.TryAdd(this, outP);

            // Update Contexts
            outP.gameContext = this;


            Logging.Write(Logging.Type.GENERAL, "Swapped Clients!");
        }

    }
}
