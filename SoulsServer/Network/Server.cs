using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Souls.Server.Chat;
using System.Collections.Concurrent;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Souls.Server.Tools;
using Souls.Server.Network;
using Souls.Server.Objects;
using Souls.Server.Game;
using SoulsServer.Network;

// https://github.com/sta/websocket-sharp#websocket-server
namespace Souls.Server.Network
{




    public abstract class Client : WebSocketService
    {
        public JToken payload { get; set; }
        public int type { get; set; }
        public Logging.Type logType { get; set; }

        public bool loggedIn { get; set; }

        public string userEndpoint { get; set; }

        public void HeartBeat()
        {
            string d = this.payload["heartbeat"].ToString();
            Response response = new Response(
                SERVICE_RESPONSE.HEARTBEAT_REPLY,
                JsonConvert.DeserializeObject(
                    "{first:" + this.payload["heartbeat"].ToString() + "," +
                    "last:" + this.payload["last"].ToString() + "}"
                )
            );

            SendTo(response);
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        public void SendError(string errorMessage)
        {
            Response response = new Response(SERVICE_RESPONSE.ERROR, errorMessage);
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
            Response response = new Response(SERVICE_RESPONSE.DEBUG, message);
            Send(response.ToJSON());
        }

        // Responses 0-99
        public enum SERVICE_RESPONSE
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


        public enum SERVICE
        {
            LOGIN = 0,
            LOGOUT = 1,
            HEARTBEAT = 2,
        }
    }



    public class Server
    {

        public Server()
        {
            var wssv = new HttpServer(8140);


            GameEngine gameEngine = new GameEngine();
            ChatEngine chatEngine = new ChatEngine();


            wssv.AddWebSocketService<GameService>("/game", () => new GameService(gameEngine));
            wssv.AddWebSocketService<ChatService>("/chat", () => new ChatService(chatEngine));

            wssv.Log.Level = LogLevel.Fatal;


            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }

        public void onConnect()
        {

        }
    }

    public class Clients
    {
        private static Clients instance;
        public ConcurrentDictionary<Client, Player> gameList { get; set; }
        public ConcurrentDictionary<Client, Player> chatList { get; set; }
        private Clients()
        {
            gameList = new ConcurrentDictionary<Client, Player>();
            chatList = new ConcurrentDictionary<Client, Player>();
        }

        public static Clients GetInstance()
        {
            if (instance == null)
            {
                instance = new Clients();
            }

            return instance;
        }

    }
}
