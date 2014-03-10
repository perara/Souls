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
using ServerWBSCKTest.Chat;


namespace ServerWBSCKTest
{
    public class SocketServer
    {

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        /// 
        public static ConcurrentDictionary<UserContext, Player> OnlinePlayers = new ConcurrentDictionary<UserContext, Player>();
        GameDataHandler gameDataHandler;
        ChatDataHandler chatDataHandler;
        public SocketServer(GameDataHandler gameDataHandler, ChatDataHandler chatDataHandler)
        {
            this.gameDataHandler = gameDataHandler;
            this.chatDataHandler = chatDataHandler;
            // Initialize the server on port 81, accept any IPs, and bind events.
            var aServer = new WebSocketServer(8140, IPAddress.Any)
            {
                //SubProtocol = new [] {"test2"};
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
                    //Response r = new Response(ResponseType.CHAT_MESSAGE, consoleInput);

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

            string data = context.DataFrame.ToString();
            try
            {
                // Fetch the JSON string and convert it to a dynamic object. (Jsonobj)
                dynamic jsonObject = JsonConvert.DeserializeObject(context.DataFrame.ToString());
                //ClientData cd = new ClientData(jsonObject);


                Service requestType = (Service)jsonObject.Service;
                dynamic serviceData = jsonObject.ServiceData;
                
                switch (requestType)
                {
                    case Service.CHAT:
                        //Authenticate(context, serviceData);
                        chatDataHandler.trafficHandler(context, serviceData);
                        break;

                    case Service.GAME:
                        //Authenticate(context, serviceData);
                        gameDataHandler.trafficHandler(context, serviceData);
                        break;

                    case Service.GENERAL:
                        if (serviceData.Type == General.LOGIN) Login(context, serviceData.Payload);
                        else if (serviceData.Type == General.LOGOUT) OnDisconnect(context);
                        break;

                }
            }
            catch (Exception exception) // Bad JSON! For shame.
            {
                var response = new Response(ResponseType.ERROR, exception.Message);
                context.Send(JsonConvert.SerializeObject(response));
            }
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        public void Login(UserContext context, dynamic data)
        {

            // Check if user is already in OnlineUsers
            if (OnlinePlayers.ContainsKey(context))
            {
                SendError(context, "Player already logged in or maybe hash is wrong or missing");
                return;
            }

            OnlinePlayers.TryAdd(context, new Player()); 
            
            // Updates the hash and loads the player info from the database if not already loaded
            OnlinePlayers[context].hash = data.hash;
            bool success = OnlinePlayers[context].fetchPlayerInfo();
            if (success) Send(context, new Response(ResponseType.AUTHENTICATED, "Logged in as " + OnlinePlayers[context].name));
            else
            {
                Player trash;
                SendError(context, "Problem fetching player info, client and server hash mismatch");
                OnlinePlayers.TryRemove(context, out trash);
            } 

            Console.WriteLine("> Client authenticated: " + context.ClientAddress);
            Console.WriteLine("Currently online players: " + OnlinePlayers.Count());

        }

        //TODO FIX THIS, retrurning else every time probably string cast
        public bool Authenticate(UserContext context, dynamic data)
        {
            if (OnlinePlayers[context].hash == (string)data.hash)
            {
                return true;
            }
            else if (data.hash == "")
            {
                SendError(context, "No hash to authenticate. Is user logged in");
                return false;
            }
            else
            {
                SendError(context, "Hash exist but didn't match registered hash for this player!");
                return false;
            }
        }

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnDisconnect(UserContext context)
        {
            bool success = false;
            try
            {
                Player trash;
                KeyValuePair<UserContext, Player> userItem = OnlinePlayers.Where(x => x.Key.ClientAddress == context.ClientAddress).FirstOrDefault();

                success = OnlinePlayers.TryRemove(userItem.Key, out trash);

                GameQueue.getInstance().removePlayer(userItem.Value);

                if (success)
                {
                    Response response = new Response(ResponseType.DISCONNECTED, "You are now logged out!");
                    Send(context, response);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                SendError(context, e.GetBaseException().ToString());
            }
            //Console.WriteLine(OnlinePlayers[context].name + " quitting");
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


        public void Send(LinkedList<UserContext> clients, Response message)
        {
            foreach (UserContext client in clients)
            {
                client.Send(JsonConvert.SerializeObject(message));
            }
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
            public dynamic Type { get; set; }
            public dynamic Data { get; set; }

            public Response(object type, dynamic data)
            {
                //if (!(type is int)) throw new NotSupportedException("Wrong responsetype");
                this.Type = type;
                this.Data = data;
            }

            public Response() { }
        }

        /// <summary>
        /// Defines the type of response to send back to the client for parsing logic
        /// </summary>
        /// 

        public enum ResponseType
        {
            AUTHENTICATED = 0,
            DISCONNECTED = 1,
            GAME = 2,
            CHAT = 3,
            ERROR = 255
        }
        public enum Service
        {
            GENERAL = 0,
            GAME = 1,
            CHAT = 2 
        }
        public enum General
        {
            LOGIN = 0,
            LOGOUT = 1
        }
        
    }
}


