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
using Souls.Server.Game;
using Souls.Server.Engine;
using Souls.Server.Tools;
using Souls.Server.Chat;
using Souls.Server.Network;
using Souls.Model.Helpers;
using NHibernate.Linq;
using SoulsServer.Objects;

namespace Souls.Server.Engine
{
    class Runner
    {
        static void Main(string[] args)
        {
            if (args.Contains("debug"))
            {
                int threadSleep = 0;

                if (args.Contains("delay"))
                    int.TryParse(args[1], out threadSleep);

                Debug();
            }

            Souls.Server.Network.Server s = new Souls.Server.Network.Server();
        }

        /// <summary>
        /// Runs integrated tests on bot and stability
        /// </summary>
        static void Debug()
        {
            Thread ct = new Thread(delegate()
            {
                //Thread.Sleep(2000);
                List<Souls.Model.Player> players = null;
                using (var session = NHibernateHelper.OpenSession())
                {

                    players = session.Query<Souls.Model.Player>().ToList();

                }

                foreach (var p in players)
                {

                    if (p.name == "BOT") continue;
                    string hash = p.GetHash();



                    if (hash == null) continue;

                    Thread bThread = new Thread(delegate()
                    {
                        AI botPlayer = new AI();
                        botPlayer.Connect(hash);
                    });
                    bThread.Start();

                    Thread.Sleep(1000);

                }
            });
            ct.Start();
        }


    }
}
