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
        int reqNum { get; set; }

        public enum AdminRequest
        {
            LOGIN = 1,
            ONLINE_USERS = 2,
            ONGOING_GAMES = 3,
            END_GAME = 4,
            BAN_USER = 5,
            KICK_USER = 6,
            RESET = 7,
        }

        public enum AdminResponse
        {
            LOGIN = 1,
            ONLINE_USERS = 2,
            ONGOING_GAMES = 3,
            END_GAME = 4,
            BAN_USER = 5,
            KICK_USER = 6,
            RESET = 7,
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

                    var pIds = from it in Clients.GetInstance().gameList.Where(x => x.Key.loggedIn == true) select new JProperty(reqNum++.ToString(), it.Value.id);

                    // Send the Online Player list
                    this.SendTo(new Response(
                        AdminResponse.ONLINE_USERS,
                        new JObject(pIds)));

                    break;
                /* case AdminRequest.ONGOING_GAMES:
                     break;
                 case AdminRequest.END_GAME:
                     break;*/
                case AdminRequest.BAN_USER:
                    int playerId = int.Parse(payload["pId"].ToString());
                    DateTime until = DateTime.Parse(payload["until"].ToString());


                    using (var session = NHibernateHelper.OpenSession())
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            PlayerBans pb = new PlayerBans();
                            pb.until = until;
                            pb.player = session.Query<Player>().Where(x => x.id == playerId).FirstOrDefault(); // Might be null

                            session.Save(pb);
                            transaction.Commit();
                        }
                    }

                    break;
                case AdminRequest.KICK_USER:
                    int pId = int.Parse(payload["pId"].ToString());

                    using (var session = NHibernateHelper.OpenSession())
                    {
                        Player p = session.Query<Player>().Where(x => x.id == pId).FirstOrDefault(); // Might be null
                        if (p != null) // If he actually exists
                        {
                            // Check if he exists in the gameList
                            KeyValuePair<Client, Objects.Player> kvp = Clients.GetInstance().gameList.Where(x => x.Value.id == p.id).FirstOrDefault();


                            // Check that the game is actually started fully!
                            if (kvp.Value == null) return;
                            if (kvp.Key == null) return;
                            if (kvp.Value.gPlayer == null) return; // Should not be null
                            if (kvp.Value.gPlayer.gameRoom == null) return; // Should not be null
                            if (kvp.Value.gPlayer.gameRoom.players.First == null) return; // Should not be null
                            if (kvp.Value.gPlayer.gameRoom.players.Second == null) return; // Should not be null

                            kvp.Value.gPlayer.gameRoom.logger.Add(GameLogger.logTypes[GameLogger.LogTypes.KICKED], pId, pId, "Player", "Player");

                            kvp.Value.gPlayer.gameRoom.winner = kvp.Value.GetOpponent();
                            kvp.Value.gPlayer.gameRoom.EndGame(false);

                            kvp.Key.Context.WebSocket.Close();
                        }

                    }
                    break;
                case AdminRequest.RESET:
                    int pid = int.Parse(payload["pId"].ToString());

                    using (var session = NHibernateHelper.OpenSession())
                    {
                        using (var transaction = session.BeginTransaction())
                        {

                            Player p = session.Query<Player>().Where(x => x.id == pid).FirstOrDefault(); // Might be null
                            List<Card> starterCards = session.Query<Card>().Where(x => x.level == 1).ToList();

                            if (p == null) return; // Should NEVER happen :D

                            p.money = 0;
                            p.rank = 1;
                            session.Save(p);

                            // Remove all cards
                            List<PlayerCards> pCards = session.Query<PlayerCards>().Where(x => x.player == p).ToList();
                            foreach (var pC in pCards)
                            {
                                session.Delete(pC);
                            }



                            // Add new starter cards
                            for (int i = 0; i < 5; i++)
                            {
                                PlayerCards pC = new PlayerCards();
                                pC.card = starterCards[i];
                                pC.obtainedat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pC.player = p;

                                session.Save(pC);
                            }


                            transaction.Commit();


                        }
                    }



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
