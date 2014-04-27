using Souls.Server.Game;
using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Chat;

namespace Souls.Server.Network
{
    public class AdminService : Service
    {

        public enum AdminRequest
        {
            LOGIN = 1,
            

        }

        public enum AdminResponse
        {

        }

        public GameEngine gEngine { get; set; }
        public ChatEngine chEngine { get; set; }


        public AdminService(GameEngine gEngine, ChatEngine chEngine)
        {
            this.gEngine = gEngine;
            this.chEngine = chEngine;
        }

        public override void Process()
        {
           


        }

        public override void Login()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }

        public override void Logout()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }




    }
}
