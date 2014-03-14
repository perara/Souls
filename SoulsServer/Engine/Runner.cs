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
using SoulsServer.Game;
using Alchemy.Classes;
using SoulsServer.Engine;
using SoulsServer.Tools;
using SoulsServer.Chat;

namespace SoulsServer
{
    class Runner
    {
        static void Main(string[] args)
        {

            //WebSocketRawTest srv = new WebSocketRawTest();


            new SocketServer();


            // Initialize GameEngine
            /*GameEngine gameEngine = new GameEngine();
            ChatEngine chatEngine = new ChatEngine();
            gameEngine.pollQueue();

            GameDataHandler gameDataHandler = new GameDataHandler(gameEngine);
            ChatDataHandler chatDataHandler = new ChatDataHandler(chatEngine);

            SocketServer srv = new SocketServer(gameDataHandler, chatDataHandler);

            gameEngine.addCallbacks((Action<Pair<GamePlayer>, SocketServer.Response>)srv.Send);
            gameEngine.addCallbacks((Action<int, SocketServer.Response>)srv.Send);
            gameEngine.addErrorCallback((Action<GamePlayer, string>)srv.SendError);

            chatEngine.addCallbacks((Action<LinkedList<UserContext>, SocketServer.Response>)srv.Send);
            

            //ChatEngine chatEngine = new ChatEngine();
            //chatEngine.addCallbacks((Action<LinkedList<UserContext>, ChatServer.Response>)chatSrv.Send);
             */
        }
    }
}
