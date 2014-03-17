using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using SoulsServer.Chat;
using System.Collections.Concurrent;
using Newtonsoft;
using SoulsServer.Model;
using Newtonsoft.Json.Linq;
using SoulsServer.Tools;
using SoulsServer.Controller;
// https://github.com/sta/websocket-sharp#websocket-server
namespace SoulsServer.Engine
{
    public class GameService : General
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
            GAME_NOT_YOUR_TURN = 202,
            GAME_NEXT_ROUND_FAIL = 203,
            GAME_AUTH_FAIL = 204,
            GAME_OPPONENT_NOEXIST = 205,
            GAME_CREATE = 206,

            GAME_USECARD_OK = 207,
            GAME_USECARD_OOM = 208,

            GAME_OPPONENT_MOVE = 209,
            GAME_OPPONENT_RELEASE = 210,
          

            GAME_RECOVER = 220 // When the client disconnected and needs a recover update.

        }


        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        private enum GameType
        {
            // Queue 
            QUEUE = 100,

            // Game
            ATTACK = 200,
            USECARD = 201,
            NEXTROUND = 202,
            MOVE_CARD = 203,
            RELEASE_CARD = 204

        }
        public GameEngine engine;

        public GameService(GameEngine engine)
        {
            this.engine = engine;
        }

        protected override void OnMessage(MessageEventArgs e)
        {

            data = JsonConvert.DeserializeObject(e.Data);
            payload = this.data.Payload;
            type = this.data.Type;


            switch ((GameType)type)
            {
                // GAME LOGIC REQUESTS
                case GameType.QUEUE:
                    engine.Request_QueuePlayer(OnlinePlayers.GetInstance().gameList[this]);
                    break;

                case GameType.ATTACK:
                    engine.Request_CardAttack(data.Payload);
                    break;

                case GameType.USECARD:
                    engine.Request_UseCard(OnlinePlayers.GetInstance().gameList[this]);
                    break;

                case GameType.NEXTROUND:
                    engine.Request_NextRound(data.Payload);
                    break;
                case GameType.MOVE_CARD:
                    engine.Request_MoveCard(OnlinePlayers.GetInstance().gameList[this]);
                    break;
                case GameType.RELEASE_CARD:
                    engine.Request_ReleaseCard(OnlinePlayers.GetInstance().gameList[this]);
                    break;

            }
            switch ((GENERAL)type)
            {
                case GENERAL.LOGIN:
                    this.GameLogin();
                    break;

                case GENERAL.LOGOUT:
                    this.Logout();
                    break;

                case GENERAL.HEARTBEAT:
                    this.HeartBeat();
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("[GAME]: Player {0} connected!", Context.UserEndPoint);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("[GAME]: Error on Player: " + e.Message.ToString());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[GAME]: Player {0} disconnected!", e.Reason);
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the gameList of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        public void GameLogin()
        {
            string hash = this.payload.hash;

            if (hash != null)
            {
                Player newPlayer = new Player();
                newPlayer.hash = this.payload.hash;
                newPlayer.gameContext = this;


                // Check if the same player tries to log in twice (Same user/hash)
                General origUser;
                if ((origUser = (OnlinePlayers.GetInstance().gameList.Where(x => x.Value.UpdateHash() == newPlayer.hash).FirstOrDefault().Key)) != null)
                {
                    Console.WriteLine("> [GAME]: User tried to log in twice");

                    // Closes the Original connection
                    //  origUser.Context.WebSocket.Close(WebSocketSharp.CloseStatusCode.NORMAL, "Same user connected twice, terminating first user");

                    // Swaps out the old Connection with the new one.
                    this.SwapOutClient(origUser);

                }
                else
                {

                    // Add the new player
                    OnlinePlayers.GetInstance().gameList.TryAdd(this, newPlayer);

                    // Updates the hash and loads the player info from the database if not already loaded
                    bool success = newPlayer.fetchPlayerInfo();
                    if (success)
                    {
                        SendTo(new Response(ResponseType.LOGIN_OK, "Logged in as " + OnlinePlayers.GetInstance().gameList[this].name));
                        Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                        Console.WriteLine("> Online players: " + OnlinePlayers.GetInstance().gameList.Count());
                    }
                    else
                    {
                        Player trash;
                        SendError("Problem fetching player info, client and server hash mismatch");
                        OnlinePlayers.GetInstance().gameList.TryRemove(this, out trash);
                        Console.WriteLine("> Client login failed for: " + Context.UserEndPoint);
                    }

                }
            }
            else
            {
                this.Context.WebSocket.Close(CloseStatusCode.UNDEFINED ,"Hash was null");
            }
        }

    }

    public class ChatService : General
    {

        /// <summary>
        /// Responses is 1000 ++
        /// </summary>
        public new enum ResponseType
        {
            // General
            CHAT_ENABLED = 1000,
            CHAT_DISABLED = 1001,
            CHAT_MESSAGE = 1003,
            CHAT_ROOM_MADE = 1004,
            INVITED_CLIENT = 1005,
            KICKED_CLIENT = 1006,
            LEFT_ROOM = 1007,
            GOT_INVITED = 1008,
            GOT_KICKED = 1009,
            MADE_LEADER = 1010,

            CHAT_NOT_MEMBER = 1096,
            NOT_LEADER = 1097,
            CLIENT_NOT_FOUND = 1098,
            CHAT_ERROR = 1099
            
        }

        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// Chat IDS = 1000++++
        /// </summary>
        public enum ChatType
        {
            ENABLE = 1000,
            DISABLE = 1001,
            MESSAGE = 1002,
            NEWROOM = 1003,
            INVITE = 1004,
            KICK = 1005,
            LEAVE = 1006,
            CHAT_LOGIN = 1007,
            CHAT_LOGOUT = 1008,
            NEWGAMEROOM = 1009,
        }

        public ChatEngine engine;

        public ChatService(ChatEngine engine)
        {
            this.engine = engine;
        }

        protected override void OnMessage(MessageEventArgs e)
        {

            data = JsonConvert.DeserializeObject(e.Data);
            payload = this.data.Payload;
            type = this.data.Type;

            // TODO: Check payload integrity, missing elements causes crash


            switch ((ChatType)type)
            {
                // CHAT REQUESTS
                case ChatType.ENABLE:
                    //engine.EnableChat(OnlinePlayers.GetInstance().gameList[this]);
                    break;

                case ChatType.DISABLE:
                    //engine.DisableChat(OnlinePlayers.GetInstance().gameList[this]);
                    break;

                case ChatType.MESSAGE:
                    engine.SendMessage(OnlinePlayers.GetInstance().gameList[this].chPlayer);
                    break;

                case ChatType.NEWROOM:
                    engine.Request_NewGameRoom(OnlinePlayers.GetInstance().gameList[this].chPlayer);
                    break;
                case ChatType.NEWGAMEROOM:

                    //////////////////////////////////////////////////////////////////////////
                    //TODO CLEANUP Should check that this.player != null


                    //Player requestPlayer = OnlinePlayers.GetInstance().chatList[this];
                    // Go via the game player object to get opponent context.
                    //Player opponentPlayer = OnlinePlayers.GetInstance().gameList[requestPlayer.gPlayer.GetOpponent().playerContext];
                    
                    //engine.Request_NewGameRoom(new Pair<ChatPlayer>(requestPlayer.chPlayer, opponentPlayer.chPlayer));

                  

                    break;
                case ChatType.INVITE:
                    engine.Invite(OnlinePlayers.GetInstance().gameList[this].chPlayer);
                    break;

                case ChatType.KICK:
                    engine.Kick(OnlinePlayers.GetInstance().gameList[this].chPlayer);
                    break;

                case ChatType.LEAVE:
                    engine.LeaveRoom(OnlinePlayers.GetInstance().gameList[this].chPlayer);
                    break;
            }

            switch ((GENERAL)this.type)
            {
                case GENERAL.LOGIN:
                    ChatLogin();
                    break;
                case GENERAL.HEARTBEAT:
                    this.HeartBeat();
                    break;

            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("[CHAT]: Player {0} connected!", Context.UserEndPoint);
        }

        protected override void OnError(ErrorEventArgs e)
        {

            Console.WriteLine("[CHAT]: Error on Player {0}" + e.Message.ToString());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[CHAT]: Player {0} disconnected!");

        }



        public void ChatLogin()
        {
            string hash = this.payload.hash;

            Console.WriteLine("Should be +1: " + OnlinePlayers.GetInstance().gameList.Count());

            KeyValuePair<General, Player> cli = OnlinePlayers.GetInstance().gameList.Where(x => x.Value.hash == hash).FirstOrDefault(); //Game Record
            if (cli.Key != null)
            {
                Console.WriteLine("> [CHAT]: Found open game connection Linking.....");
          
                Player existingPlayer = cli.Value;

                // Insert the chatConnection record
                OnlinePlayers.GetInstance().chatList.TryAdd(this, existingPlayer);

                // Update the playerObject contexts
                existingPlayer.chatContext = this;

                // Create a new chatPlayer
                if (existingPlayer.chPlayer == null)
                {
                    ChatPlayer chPlayer = new ChatPlayer(existingPlayer.name);
                    existingPlayer.chPlayer = chPlayer;
                    chPlayer.chatContext = existingPlayer.chatContext;
                }

            }
            else // No existing game Player was found, create new player
            {
                 // General function for creating player
            }




        }
    }

    public abstract class General : WebSocketService
    {

        public dynamic data { get; set; }
        public dynamic payload { get; set; }
        public int type { get; set; }

        public void Logout()
        {
            bool success = false;
            try
            {

                GameQueue.GetInstance().removePlayer(OnlinePlayers.GetInstance().gameList[this]);

                Player trash;
                success = OnlinePlayers.GetInstance().gameList.TryRemove(this, out trash);

                if (success)
                {
                    Response response = new Response(ResponseType.DISCONNECTED, "You are now logged out!");
                    SendTo(response);
                    Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                    Console.WriteLine("> Online players: " + OnlinePlayers.GetInstance().gameList.Count());

                }
            }
            catch (Exception exception) // Bad JSON! For shame.
            {
                var response = new Response(ResponseType.ERROR, exception.Message);
                SendTo(response);
            }
        }

        /// <summary>
        /// Swaps out a client with another in the OnlinePlayers gameList
        /// </summary>
        public void SwapOutClient(General swapOut)
        {

            Player outP;

            // Gets the Player object
            OnlinePlayers.GetInstance().gameList.TryRemove(swapOut, out outP);

            OnlinePlayers.GetInstance().gameList.TryAdd(this, outP);

            // Update Contexts
            outP.gameContext = this;
            if (outP.gPlayer != null)
            {
                outP.gPlayer.playerContext = this;
            }



            Console.WriteLine("> [GENERAL] Swapped out the General Object!");
        }

        public void HeartBeat()
        {
            string d = this.payload.heartbeat;
            Response response = new Response(ResponseType.HEARTBEAT_REPLY, JsonConvert.DeserializeObject("{first:" + this.payload.heartbeat + ", last:" + this.payload.last + "}"));
            SendTo(response);
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        public void SendError(string errorMessage)
        {
            Response response = new Response(ResponseType.ERROR, errorMessage);
            SendTo(response);
        }

        /// <summary>
        /// This function sends a response to the Player context
        /// </summary>
        /// <param name="json">Takes in a JSON string</param>

        public void SendTo(Response response)
        {
            Send(response.ToJSON());
        }
        public void SendDebug(string message)
        {
            Response response = new Response(ResponseType.DEBUG, message);
            Send(response.ToJSON());
        }

        // Responses 0-99
        public enum ResponseType
        {
            DISCONNECTED = 1,
            HEARTBEAT_REPLY = 5,
            DEBUG = 99,
            ERROR = 98,

            LOGIN_OK = 10,
            LOGIN_NO_HASH = 11,
            LOGIN_WRONG_HASH = 12,
            LOGIN_NOT_LOGGED_IN = 13,
        }


        public enum GENERAL
        {
            LOGIN = 0,
            LOGOUT = 1,
            HEARTBEAT = 2,
        }
    }



    public class SocketServer
    {

        public SocketServer()
        {
            var wssv = new HttpServer(8140);


            GameEngine gameEngine = new GameEngine();
            ChatEngine chatEngine = new ChatEngine();


            wssv.AddWebSocketService<GameService>("/game", () => new GameService(gameEngine));
            wssv.AddWebSocketService<ChatService>("/chat", () => new ChatService(chatEngine));


            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }

        public void onConnect()
        {

        }
    }

    public class OnlinePlayers
    {
        private static OnlinePlayers instance;
        public ConcurrentDictionary<General, Player> gameList { get; set; }
        public ConcurrentDictionary<General, Player> chatList { get; set; }
        private OnlinePlayers()
        {
            gameList = new ConcurrentDictionary<General, Player>();
            chatList = new ConcurrentDictionary<General, Player>();
        }

        public static OnlinePlayers GetInstance()
        {
            if (instance == null)
            {
                instance = new OnlinePlayers();
            }

            return instance;
        }

    }
}
