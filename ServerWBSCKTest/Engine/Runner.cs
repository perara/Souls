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
using ServerWBSCKTest.Game;
using Alchemy.Classes;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;

namespace ServerWBSCKTest
{
    class Runner
    {
        static void Main(string[] args)
        {



            // Initialize GameEngine
            GameEngine engine = new GameEngine();
            engine.pollQueue();

            Server srv = new Server(engine);
            engine.addCallbacks((Action<Pair<GamePlayer>, Server.Response>)srv.Send);
            engine.addCallbacks((Action<int, Server.Response>)srv.Send);
            engine.addErrorCallback((Action<GamePlayer, string>)srv.SendError);
       
        }
    }
}
