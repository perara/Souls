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
using SoulsServer.Engine;
using SoulsServer.Tools;
using SoulsServer.Chat;
using SoulsServer.Network;

namespace SoulsServer.Engine
{
    class Runner
    {
        static void Main(string[] args)
        {
            Server s = new Server();
        }
    }
}
