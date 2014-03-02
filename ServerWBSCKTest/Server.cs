using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace ServerWBSCKTest
{
    class Server
    {

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        /// 
        protected static ConcurrentDictionary<Player, UserContext> OnlinePlayers = new ConcurrentDictionary<Player, UserContext>();

        public Server()
        {
            // Initialize the server on port 81, accept any IPs, and bind events.
            var aServer = new WebSocketServer(8140, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            GameEngine engine = new GameEngine();
            engine.pollQueue();


            aServer.Start();

            // Accept commands on the console and keep it alive
            var command = string.Empty;
            while (command != "exit")
            {
                command = Console.ReadLine();
                Response r = new Response();
                r.Data = command;
                r.Type = ResponseType.Message;
                Broadcast(command, OnlinePlayers.Values);
            }

            aServer.Stop();
        }

        /// <summary>
        /// Event fired when a client connects to the Alchemy Websockets server instance.
        /// Adds the client to the online users list.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public static void OnConnect(UserContext context)
        {
            Console.WriteLine();
            Console.WriteLine("Client Connection From : " + context.ClientAddress);
        }

        /// <summary>
        /// Event fired when a data is received from the Alchemy Websockets server instance.
        /// Parses data as JSON and calls the appropriate message or sends an error message.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public static void OnReceive(UserContext context)
        {
            Console.WriteLine("Received Data From :" + context.ClientAddress);

            Console.WriteLine(context.DataFrame.ToString());

            try
            {
                var json = context.DataFrame.ToString();

                // <3 dynamics

                dynamic obj = JsonConvert.DeserializeObject(json);

                switch ((int)obj.Type)
                {
                    case (int)CommandType.Register:
                        Register(context, obj.Data);
                        Console.WriteLine("Registered user");
                        break;
                    case (int)CommandType.Login:
                        //Register(context, obj.Data); //_----------------------------------------
                        Console.WriteLine("User logged in");
                        //ChatMessage(obj.Message.Value, context);
                        break;
                }
                Console.Write(obj);
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new Response { Type = ResponseType.Error, Data = new { e.Message } };

                context.Send(JsonConvert.SerializeObject(r));
            }
        }

        public static void SendTo(int pId, Response message)
        {
            UserContext context = OnlinePlayers.Where(x => x.Key.id == pId).First().Value;
            context.Send(JsonConvert.SerializeObject(message));
        }

        public static void SendTo(UserContext context, Response message)
        {
            context.Send(JsonConvert.SerializeObject(message));
        }

        /// <summary>
        /// Event fired when the Alchemy Websockets server instance sends data to a client.
        /// Logs the data to the console and performs no further action.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public static void OnSend(UserContext context)
        {
            Console.WriteLine("Data Send To : " + context.ClientAddress);
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        private static void Register(UserContext context, dynamic obj)
        {
            // Create the player object
            Player p = QueuePool.getInstance().addPlayer(obj);
            OnlinePlayers.TryAdd(p, context);

            var response = new Response();
            response.Type = ResponseType.Connection;
            response.Data = new { p.id };

            SendTo(context, response);
            //Broadcast(JsonConvert.SerializeObject(response));

        }


        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public static void OnDisconnect(UserContext context)
        {
            Console.WriteLine("Client Disconnected : " + context.ClientAddress);
            try
            {
                KeyValuePair<Player, UserContext> playerItem = OnlinePlayers.Where(o => o.Value.ClientAddress == context.ClientAddress).Single();
                int reData = (playerItem.Key == null) ? playerItem.Key.id : -1;

                UserContext trash; // Concurrent dictionaries make things weird

                OnlinePlayers.TryRemove(playerItem.Key, out trash);
                QueuePool.getInstance().removePlayer(playerItem.Key);

                var response = new Response();
                response.Type = ResponseType.Disconnect;
                response.Data = new { reData };

            }
            catch(System.InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
                SendError(e.ToString(), context);
            }

            Console.WriteLine(OnlinePlayers.Count());
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        private static void SendError(string errorMessage, UserContext context)
        {
            var r = new Response { Type = ResponseType.Error, Data = new { Message = errorMessage } };

            context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Broadcasts a message to all users, or if users is populated, a select list of users
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        /// <param name="users">Optional list of users to broadcast to. If null, broadcasts to all. Defaults to null.</param>
        private static void Broadcast(string message, ICollection<UserContext> users = null)
        {

            if (users == null)
            {
                foreach (var u in OnlinePlayers.Values)
                {
                    u.Send(message);
                }
            }
            else
            {
                foreach (var u in OnlinePlayers.Values.Where(users.Contains))
                {
                    u.Send(message);
                }
            }
        }

        /// <summary>
        /// Defines the type of response to send back to the client for parsing logic
        /// </summary>
        public enum ResponseType
        {
            Connection = 0,
            Disconnect = 1,
            Message = 2,
            NameChange = 3,
            UserCount = 4,
            RegAccept = 5,
            Error = 255
        }

        /// <summary>
        /// Defines the response object to send back to the client
        /// </summary>
        public class Response
        {
            public ResponseType Type { get; set; }
            public dynamic Data { get; set; }
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
        /// Defines a type of command that the client sends to the server
        /// </summary>
        public enum CommandType
        {
            Register = 0,
            Login = 1,
            Logout = 2,
            Message,
        }
    }
}


