using Souls.Server.Game;
using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Chat;
using Souls.Model.Helpers;
using NHibernate.Linq;
using Souls.Model;
using Newtonsoft.Json.Linq;

namespace Souls.Server.Network
{
    public class AdminService : Service
    {

        public enum AdminRequest
        {
            LOGIN = 1,
            ONLINE_USERS = 2,
            ONGOING_GAMES = 3,
            END_GAME = 4,
            BAN_USER = 5
        }

        public enum AdminResponse
        {
            LOGIN = 1,
            ONLINE_USERS = 2,
            ONGOING_GAMES = 3,
            END_GAME = 4,
            BAN_USER = 5
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

            if (!this.loggedIn && (AdminRequest)this.type != AdminRequest.LOGIN) return;

            switch ((AdminRequest)type)
            {
                case AdminRequest.LOGIN:
                    this.Login();
                    break;
                case AdminRequest.ONLINE_USERS:


                    var pIds = from it in Clients.GetInstance().gameList select new JProperty("id",it.Value.id);

                   

                    // Send the Online Player list
                    this.SendTo(new Response(
                        AdminResponse.ONLINE_USERS,
                        new JObject(pIds)));

                    break;
                case AdminRequest.ONGOING_GAMES:
                    break;
                case AdminRequest.END_GAME:
                    break;
                case AdminRequest.BAN_USER:
                    break;

            }



        }

        public override void Login()
        {
            string hash = this.payload["hash"].ToString();


            using (var session = NHibernateHelper.OpenSession())
            {
                PlayerLogin validLogin = session.Query<PlayerLogin>()
                    .Where(x => x.hash == hash)
                    .FirstOrDefault();

                // Login failed
                if (validLogin == null)
                {
                    // Login failed
                    this.loggedIn = false;
                    return;
                }


                // Login OK Beyond this point
                this.loggedIn = true;

            }








        }

        public override void Logout()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }




    }
}
