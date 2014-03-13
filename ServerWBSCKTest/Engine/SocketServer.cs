using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using ServerWBSCKTest.Chat;
using System.Collections.Concurrent;
using Newtonsoft;
using ServerWBSCKTest.Model;
// https://github.com/sta/websocket-sharp#websocket-server
namespace ServerWBSCKTest.Engine
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
        }
        public GameEngine engine;

        public GameService(GameEngine engine)
        {
            this.engine = engine;
        }

        protected override void OnMessage(MessageEventArgs e)
        {

            // Parse JSON string to dynamic object
            this.data = JsonConvert.DeserializeObject(e.Data);
            this.payload = this.data.Payload;

            Console.WriteLine(data);

            switch ((int)data.Type)
            {
                // GAME LOGIC REQUESTS
                case (int)GameType.QUEUE:
                    if (Authenticate(this))
                        engine.QueuePlayer(OnlinePlayers[this]);
                    break;

                case (int)GameType.ATTACK:
                    if (Authenticate(this))
                    {
                        engine.RequestCardAttack(data.Payload);
                        
                    }
                    break;

                case (int)GameType.USECARD:
                    if (Authenticate(this))
                    {
                        engine.UseCardRequest(OnlinePlayers[this]);
                       
                    }
                    break;

                case (int)GameType.NEXTROUND:
                    if (Authenticate(this))
                    {
                        engine.NextRoundRequest(data.Payload);
                    }
                    break;

                case (int)GENERAL.LOGIN:
                    this.Login();
                    break;
                case (int)GENERAL.LOGOUT:
                    if (Authenticate(this))
                    {
                        this.Logout();
                    }
                    break;
                case (int)GENERAL.HEARTBEAT:
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
        public new enum ResponseType
        {
            // General
            MESSAGE = 3,
            ERROR = 25
        }

        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        public enum ChatType
        {
            ACTIVATE = 0,
            DEACTIVATE = 1,
            MESSAGE = 2,
            NEWROOM = 3,
            INVITE = 4,
            KICK = 5,
            LEAVE = 6
        }

        public ChatEngine engine;

        public ChatService(ChatEngine engine)
        {
            this.engine = engine;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            this.data = JsonConvert.DeserializeObject(e.Data);
            this.payload = this.data.Payload;

            bool success;
            int room;
            string name;

            switch ((ChatType)data.Type)
            {

                // CHAT REQUESTS
                case ChatType.ACTIVATE:
                    General.OnlinePlayers[this].chatActive = true;

                    Console.WriteLine("User \"" + General.OnlinePlayers[this].name + "\" logged in to chat");
                    break;

                case ChatType.DEACTIVATE:
                    General.OnlinePlayers[this].chatActive = false;

                    Console.WriteLine(General.OnlinePlayers[this].name + " exited chat");
                    break;

                case ChatType.MESSAGE:
                    room = payload.room;

                    string message = OnlinePlayers[this].name + ": " + payload.message;
                    Response response = new Response(ResponseType.MESSAGE, message);
                    engine.chatRooms[room].Broadcast(response);

                    Console.WriteLine(OnlinePlayers[this].name + ": " + payload.message);
                    break;

                case ChatType.NEWROOM:
                    engine.addChatRoom(OnlinePlayers[this]);
                    break;

                case ChatType.INVITE:
                    room = payload.room;

                    // Checks if client is leader
                    if (!engine.chatRooms[room].isLeader(OnlinePlayers[this]) || !engine.chatRooms[room].isStatic)
                    {
                        Console.WriteLine(General.OnlinePlayers[this].name + " tried to invite without being leader");
                        return;
                    }

                    Player invited = OnlinePlayers.Where(x => x.Value.name == (string)payload.name).FirstOrDefault().Value;
                    success = engine.chatRooms[room].AddClient(invited);

                    if (success) Console.WriteLine(OnlinePlayers[this].name + " invited " + payload.name + " to room: " + room);
                    else Console.WriteLine("Problem adding " + payload.name + " to room " + room + ". Already in room?");
                    break;

                case ChatType.KICK:
                    room = payload.room;
                    name = payload.name;

                    // Checks if client is leader
                    if (!engine.chatRooms[room].isLeader(OnlinePlayers[this]) || !engine.chatRooms[room].isStatic)
                    {
                        Console.WriteLine(OnlinePlayers[this].name + " tried to kick without being leader");
                        return;
                    }

                    Player kick = OnlinePlayers.FirstOrDefault(x => x.Value.name == (string)payload.name).Value;
                    success = engine.chatRooms[room].RemoveClient(kick);

                    if (success) Console.WriteLine(OnlinePlayers[this].name + " kicked " + payload.name + " from room: " + room);
                    else Console.WriteLine("Problem kicking " + payload.name + " from room " + room + ". Client not in room?");
                    break;

                case ChatType.LEAVE:
                    room = payload.room;

                    engine.chatRooms[room].RemoveClient(OnlinePlayers[this]);
                    if (engine.chatRooms[room].clients.Count() == 0) engine.chatRooms.Remove(room);

                    Console.WriteLine(payload.name + " left room: " + room);
                    break;
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("[CHAT]: Player {0} connected!", Context.UserEndPoint);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("[CHAT]: Error on Player {0}", Context.UserEndPoint);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("[CHAT]: Player {0} disconnected!", Context.UserEndPoint);
        }

    }

    public abstract class General : WebSocketService
    {
        public static ConcurrentDictionary<General, Player> OnlinePlayers = new ConcurrentDictionary<General, Player>();

        public dynamic data { get; set; }
        public dynamic payload { get; set; }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        public void Login()
        {

            if (this.payload.hash != null)
            {
                Player newPlayer = new Player();
                newPlayer.SessionID = this.ID;
                newPlayer.hash = this.payload.hash;
                newPlayer.playerContext = this;

                // Check if user is already in OnlineUsers //TODO WHAT IF ANOTHER CLIENT CONNECTS?

              /*  if ((OnlinePlayers.Where(x => x.Value.hash == newPlayer.hash).FirstOrDefault().Key) != null)
                {                


                    SendError("Player already logged in or maybe hash is wrong or missing");
                    return;
                }*/

                // Add the new player
                OnlinePlayers.TryAdd(this, newPlayer);

                // Updates the hash and loads the player info from the database if not already loaded
                bool success = newPlayer.fetchPlayerInfo();
                if (success)
                {
                    SendTo(new Response(ResponseType.AUTHENTICATED, "Logged in as " + OnlinePlayers[this].name));
                }
                else
                {
                    Player trash;
                    SendError("Problem fetching player info, client and server hash mismatch");
                    OnlinePlayers.TryRemove(this, out trash);
                }

                Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                Console.WriteLine("> Online players: " + OnlinePlayers.Count());
            }
        }

        public void Logout()
        {
            bool success = false;
            try
            {

                GameQueue.GetInstance().removePlayer(OnlinePlayers[this]);

                Player trash;
                success = OnlinePlayers.TryRemove(this, out trash);

                if (success)
                {
                    Response response = new Response(ResponseType.DISCONNECTED, "You are now logged out!");
                    SendTo(response);
                    Console.WriteLine("> Client authenticated: " + Context.UserEndPoint);
                    Console.WriteLine("> Online players: " + OnlinePlayers.Count());

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
            if (OnlinePlayers.ContainsKey(this))
            {

                //  Console.WriteLine(data.hash);
                string hash = context.payload.hash;
                if (OnlinePlayers[this].hash.Equals(hash))
                {
                    return true;
                }
                else if (data.hash == "")
                {
                    SendError("> Hash is NULL");
                    return false;
                }
                else
                {
                    SendError("> Hash was WRONG!");
                    return false;
                }
            }
            else
            {
                SendError("> User is not logged in!");
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

        public enum ResponseType
        {
            AUTHENTICATED = 0,
            DISCONNECTED = 1,
            HEARTBEAT_REPLY = 5,
            DEBUG = 254,
            ERROR = 255,
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
}
