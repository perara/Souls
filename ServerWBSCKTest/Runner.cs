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

namespace ServerWBSCKTest
{
    class Runner
    {
        static void Main(string[] args)
        {
 

            // Initialize GameEngine
            GameEngine engine = new GameEngine();
            engine.pollQueue();
            
            // Initialize the server
            Server srv = new Server();
        }
    }
}
