using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Threading;
using ServerWBSCKTest.Tools;
using ServerWBSCKTest.Engine;

namespace ServerWBSCKTest
{
    class Server
    {

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        /// 
        public static ConcurrentDictionary<UserContext, Player> OnlinePlayers = new ConcurrentDictionary<UserContext, Player>();
        private GameEngine gameEngine;


        public Server(GameEngine gameEngine)
        {
            this.gameEngine = gameEngine;


            // Initialize the server on port 81, accept any IPs, and bind events.
            var aServer = new WebSocketServer(8140, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            aServer.Start();

            // Create a new thread for console input
            Thread serverThread = new Thread(() =>
            {

                var consoleInput = string.Empty;
                while (consoleInput != "exit")
                {
                    // Read the userInput
                    consoleInput = Console.ReadLine();

                    // Create a response for the client
                    Response r = new Response(ResponseType.CHAT_MESSAGE, consoleInput);

                    Broadcast(consoleInput, OnlinePlayers.Keys);
                }

                // Stop the server when "exit" is entered //TODO
                aServer.Stop();
            });
            serverThread.Start();
        }


        /// <summary>
        /// Event fired when a client connects to the Alchemy Websockets server instance.
        /// Adds the client to the online users list.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnConnect(UserContext context)
        {
            OnlinePlayers.TryAdd(context, new Player());
            Console.Write("Client connected");
        }

        /// <summary>
        /// Event fired when a data is received from the Alchemy Websockets server instance.
        /// Parses data as JSON and calls the appropriate message or sends an error message.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnReceive(UserContext context)
        {
            Console.WriteLine("> Data from :" + context.ClientAddress);

            try
            {

                // Fetch the JSON string and convert it to a dynamic object. (Jsonobj)
                dynamic jsonObject = JsonConvert.DeserializeObject(context.DataFrame.ToString());

                ClientType requestType = (ClientType)jsonObject.Type;
                dynamic requestPayload = jsonObject.Payload;

                switch (requestType)
                {
                    // NON GAME LOGIC REQUESTS
                    case ClientType.REQUEST_AUTHENTICATE:
                        this.Authenticate(context, requestPayload);
                        Console.WriteLine("User \"" + OnlinePlayers[context].name + "\" logged in");
                        break;

                    case ClientType.LOGOUT:
                        OnDisconnect(context);
                        Console.WriteLine("User quit");
                        break;

                    // GAME LOGIC REQUESTS
                    case ClientType.REQUEST_QUEUE:
                        gameEngine.QueuePlayer(OnlinePlayers[context]);
                        Console.WriteLine("\t\t\t\t\t\t\t" + context.ClientAddress + " queued!");
                        break;
                    case ClientType.REQUEST_USECARD:
                        gameEngine.RequestUseCard(requestPayload);
                        Console.WriteLine(requestPayload);

                        break;
                    case ClientType.REQUEST_NEXTROUND:
                        gameEngine.RequestNextRound(requestPayload);
                        break;
                    case ClientType.REQUEST_ATTACK:
                        gameEngine.RequestCardAttack(requestPayload);
                        Console.WriteLine(requestPayload);
                        break;
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new Response(ResponseType.ERROR, e.Message);
                /*{ 
                    Type = ResponseType.Error, 
                    Data = e.Message,
                };*/

                context.Send(JsonConvert.SerializeObject(r));
            }
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        private void Authenticate(UserContext context, dynamic data)
        {
            if (data.hash == "")
            {
                SendError(context, "User is not logged in!");
                return;
            }

            OnlinePlayers[context].hash = data.hash;
            OnlinePlayers[context].fetchPlayerInfo();

            var response = new Response(ResponseType.AUTHENTICATE_OK, OnlinePlayers[context].name);

            Send(context, response);

            Console.WriteLine("> Client connected: " + context.ClientAddress);
            Console.WriteLine("Currently online players: " + OnlinePlayers.Count());

            Send(context, new Response(ResponseType.CONNECTION, "Connection Successfull"));
        }

        // Add player to game queue
        /* private void Queue(UserContext context)
         {
             gameEngine.addPlayer(OnlinePlayers[context]);
         }*/

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnDisconnect(UserContext context)
        {
            try
            {
                Player trash;
                KeyValuePair<UserContext, Player> userItem = OnlinePlayers.Where(x => x.Key.ClientAddress == context.ClientAddress).FirstOrDefault();

                int reData = (userItem.Value != null) ? userItem.Value.id : -1;
                try
                {
                    OnlinePlayers.TryRemove(userItem.Key, out trash);
                }
                catch (System.ArgumentNullException e)
                {
                    Send(context, new Response(Server.ResponseType.ERROR, e.GetBaseException().ToString()));
                }

                gameEngine.gameQueue.removePlayer(userItem.Value);

                var response = new Response(ResponseType.DISCONNECT, reData);
            }
            catch (System.InvalidOperationException e)
            {
                Console.WriteLine(e.ToString());
                SendError(context, e.ToString());
            }
            Console.WriteLine("Disconnected: " + context.ClientAddress);
            Console.WriteLine("Currently online players: " + OnlinePlayers.Count());
        }

        // Send response to player using usercontext
        public void Send(UserContext context, Response message)
        {
            context.Send(JsonConvert.SerializeObject(message));
        }

        // Send response to specific player id
        public void Send(int pId, Response message)
        {
            UserContext context = OnlinePlayers.Where(x => x.Value.id == pId).First().Key;
            context.Send(JsonConvert.SerializeObject(message));
        }


        // Send response to player using usercontext
        public void Send(Pair<GamePlayer> players, Response message)
        {
            UserContext context;
            context = OnlinePlayers.Where(x => x.Value.id == players.First.id).First().Key;
            context.Send(JsonConvert.SerializeObject(message));
            context = OnlinePlayers.Where(x => x.Value.id == players.Second.id).First().Key;
            context.Send(JsonConvert.SerializeObject(message));
        }


        /// <summary>
        /// Event fired when the Alchemy Websockets server instance sends data to a client.
        /// Logs the data to the console and performs no further action.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        private void OnSend(UserContext context)
        {
            Console.WriteLine("> Data sent: " + context.ClientAddress);
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        public void SendError(UserContext context, string errorMessage)
        {
            Response r = new Response(ResponseType.ERROR, errorMessage);
            context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        public void SendError(GamePlayer player, string errorMessage)
        {
            UserContext context;
            context = OnlinePlayers.Where(x => x.Value.id == player.id).First().Key;

            Response r = new Response(ResponseType.ERROR, errorMessage);
            context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Broadcasts a message to all users, or if users is populated, a select list of users
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        /// <param name="users">Optional list of users to broadcast to. If null, broadcasts to all. Defaults to null.</param>
        public void Broadcast(string message, ICollection<UserContext> users = null)
        {

            if (users == null)
            {
                foreach (var u in OnlinePlayers.Keys)
                {
                    u.Send(message);
                }
            }
            else
            {
                foreach (var u in OnlinePlayers.Keys.Where(users.Contains))
                {
                    u.Send(message);
                }
            }
        }

        /// <summary>
        /// Defines the response object to send back to the client
        /// </summary>
        public class Response
        {
            public ResponseType Type { get; set; }
            public dynamic Data { get; set; }

            public Response(ResponseType type, dynamic data)
            {
                this.Type = type;
                this.Data = data;
            }

            public Response() { }
        }

        /// <summary>
        /// Holds the name and context instance for an online user
        /// </summary>
        public class User
        {
            public string Name = String.Empty;
            public UserContext Context { get; set; }
        }


        /// <summary>
        /// Defines the type of response to send back to the client for parsing logic
        /// </summary>
        public enum ResponseType
        {
            // Authentication
            AUTHENTICATE_OK = 0,
            AUTHENTICATE_ERROR = 1,

            // Queue
            QUEUED_OK = 3,
            QUEUED_ERROR = 4,

            // Game
            GAME_UPDATE = 2,
            GAME_NOT_ENOUGH_MANA = 5,

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
        public enum ClientType
        {
            // Pre Player Stage
            REQUEST_AUTHENTICATE = 1,

            // Game Stage 
            REQUEST_QUEUE = 3,
            REQUEST_ATTACK = 4,
            REQUEST_USECARD = 5,
            REQUEST_NEXTROUND = 6,

            // Post Game Stage
            LOGOUT = 2,



        }
    }
}


