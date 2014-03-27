using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;

namespace SoulsClient.Controllers
{
    public class BaseController : Controller
    {


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

           
            /*
            // Only if player is logged in!
            if (cSession.Current.playerId != -1)
            {
                
                // This is a heavy operation.
                using (var db = new soulsEntities())
                {

                    
                    var playerLogin = db.db_Player_Login.FirstOrDefault(x => x.fk_player_id == cSession.Current.playerId);

                    if (playerLogin == null)
                    {

                        playerLogin = new db_Player_Login();
                        playerLogin.ip = "127.0.0.1";

                    }
                    playerLogin.logged_in_at = Toolkit.getTimestamp();

                    string newHash = Toolkit.sha256_hash(playerLogin.logged_in_at + playerLogin.fk_player_id + playerLogin.ip);

                    var playerHash = db.db_Player_Hash.FirstOrDefault(x => x.hash == cSession.Current.hash);
                    playerHash.hash = newHash;

                    cSession.Current.hash = newHash;


                  


                    db.SaveChanges();


                };
            }*/
        }


    }
}
