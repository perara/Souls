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
            GAME_OPPONENT_RELEASE = 210

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


            // TODO: Check payload integrity, missing elements causes crash



            if (type != (int)GENERAL.LOGIN && type != (int)GENERAL.HEARTBEAT)
            {
                if (!Authenticate(this)) return;
            }

            switch ((GameType)type)
            {
                // GAME LOGIC REQUESTS
                case GameType.QUEUE:
                    engine.QueuePlayer(OnlinePlayers.GetInstance().list[this]);
                    break;

                case GameType.ATTACK:
                    engine.RequestCardAttack(data.Payload);
                    break;

                case GameType.USECARD:
                    engine.UseCardRequest(OnlinePlayers.GetInstance().list[this]);
                    break;

                case GameType.NEXTROUND:
                    engine.NextRoundRequest(data.Payload);
                    break;
                case GameType.MOVE_CARD:
                    engine.MovedCard(OnlinePlayers.GetInstance().list[this]);
                    break;
                case GameType.RELEASE_CARD:
                    engine.ReleasedCard(OnlinePlayers.GetInstance().list[this]);
                    break;

            }

            switch ((GENERAL)type)
            {
                case GENERAL.LOGIN:
                    this.Login();
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
            Console.WriteLine("[GAME]: Error on Player {0}", "UNKNOWN :(");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[GAME]: Player {0} disconnected!", e.Reason);
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

            NOT_LEADER = 1097,
            CLIENT_NOT_FOUND = 1098,
            CHAT_ERROR = 1099,
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

            if (type != (int)ChatType.CHAT_LOGIN && type != (int)GENERAL.HEARTBEAT)
            {
                if (!Authenticate(this)) return;
            }

            switch ((ChatType)type)
            {
                // CHAT REQUESTS
                case ChatType.ENABLE:
                    engine.EnableChat(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.DISABLE:
                    engine.DisableChat(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.MESSAGE:
                    engine.SendMessage(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.NEWROOM:
                    engine.AddChatRoom(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.INVITE:
                    engine.Invite(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.KICK:
                    engine.Kick(OnlinePlayers.GetInstance().list[this]);
                    break;

                case ChatType.LEAVE:
                    engine.LeaveRoom(OnlinePlayers.GetInstance().list[this]);
                    break;
                case ChatType.CHAT_LOGIN:
                    engine.ChatLogin(this);
                    break;
                case ChatType.CHAT_LOGOUT:
                    engine.ChatLogout(this);
                    break;

            }

            switch ((GENERAL)this.type)
            {
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

    }

    public abstract class General : WebSocketService
    {

        public dynamic data { get; set; }
        public dynamic payload { get; set; }
        public int type { get; set; }

        public General friendCon { get; set; }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        public void Login()
        {

            string hash = this.payload.hash;




            if (hash != "null")
            {
                Player newPlayer = new Player();
                newPlayer.SessionID = this.ID;
                newPlayer.hash = this.payload.hash;
                newPlayer.context = this;
                newPlayer.chatActive = true;


               /* if ((OnlinePlayers.GetInstance().list.Where(x => x.Value.hash == newPlayer.hash).FirstOrDefault().Key) != null)
                {
                    Console.WriteLine("Someone here :)");
                }*/



                // Add the new player
                OnlinePlayers.GetInstance().list.TryAdd(this, newPlayer);

                // Updates the hash and loads the player info from the database if not already loaded
                bool success = newPlayer.fetchPlayerInfo();
                if (success)
                {
                    SendTo(new Response(ResponseType.AUTHENTICATED, "Logged in as " + OnlinePlayers.GetInstance().list[this].name));
                    Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                    Console.WriteLine("> Online players: " + OnlinePlayers.GetInstance().list.Count());
                }
                else
                {
                    Player trash;
                    SendError("Problem fetching player info, client and server hash mismatch");
                    OnlinePlayers.GetInstance().list.TryRemove(this, out trash);
                    Console.WriteLine("> Client login failed for: " + Context.UserEndPoint);
                }
            }
        }

        public void Logout()
        {
            bool success = false;
            try
            {

                GameQueue.GetInstance().removePlayer(OnlinePlayers.GetInstance().list[this]);

                Player trash;
                success = OnlinePlayers.GetInstance().list.TryRemove(this, out trash);

                if (success)
                {
                    Response response = new Response(ResponseType.DISCONNECTED, "You are now logged out!");
                    SendTo(response);
                    Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                    Console.WriteLine("> Online players: " + OnlinePlayers.GetInstance().list.Count());

                }
            }
            catch (Exception exception) // Bad JSON! For shame.
            {
                var response = new Response(ResponseType.ERROR, exception.Message);
                SendTo(response);
            }
        }



        public void HeartBeat()
        {
            string d = this.payload.heartbeat;
            Response response = new Response(ResponseType.HEARTBEAT_REPLY, JsonConvert.DeserializeObject("{first:" + this.payload.heartbeat + ", last:" + this.payload.last + "}"));
            SendTo(response);
        }

        //TODO FIX THIS, retrurning else every time probably string cast
        public bool Authenticate(General context)
        {

            // Check if the player actually exists
            if (OnlinePlayers.GetInstance().list.ContainsKey(this))
            {

                //  Console.WriteLine(data.hash);
                string hash = context.payload.hash;
                if (OnlinePlayers.GetInstance().list[this].hash.Equals(hash))
                {
                    return true;
                }
                else if (data.hash == "")
                {

                    SendTo(new Response(ResponseType.LOGIN_NO_HASH, "No hash were given (Not logged in?)"));
                    return false;
                }
                else
                {
                    SendTo(new Response(ResponseType.LOGIN_WRONG_HASH, "Hash did not match, Wannabe hacker?"));
                    return false;
                }
            }
            else
            {
                SendTo(new Response(ResponseType.LOGIN_NOT_LOGGED_IN, "You are not logged in"));
                return false;
            }
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
            AUTHENTICATED = 0,
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
        public ConcurrentDictionary<General, Player> list { get; set; }
        private OnlinePlayers()
        {
            list = new ConcurrentDictionary<General, Player>();
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
