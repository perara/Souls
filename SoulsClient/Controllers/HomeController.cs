using Souls.Client.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using Souls.Model;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Souls.Model.Helpers;

namespace SoulsClient.Controllers
{
    public class HomeController : BaseController
    {

        [AllowAnonymousAttribute]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";



            return View();
        }


        [AllowAnonymousAttribute]
        public ActionResult ServiceStatus()
        {
            // Check connectivity to services
            bool gameService = Toolkit.PingHost("tux.persoft.no", 8140);
            bool mySql = Toolkit.PingHost("persoft.no", 6001);
            Dictionary<string, bool> services = new Dictionary<string, bool>();
            services.Add("Game Server: ", gameService);
            services.Add("Database: ", mySql);


            return Json(services, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymousAttribute]
        public ActionResult GetGames()
        {

            List<dynamic> objs;
            using (var session = NHibernateHelper.OpenSession())
            {
                objs = new List<dynamic>();

                List<Game> games = session.Query<Game>()
                     .Fetch(x => x.player1)
                     .Fetch(x => x.player2)
                     .ToList();

                foreach (var item in games)
                {


                    dynamic data = new
                    {
                        gameId = item.id,
                        player1 = item.player1.name,
                        player2 = item.player2.name
                    };

                    objs.Add(data);
                }
            }

            return Json(objs, JsonRequestBehavior.AllowGet);

        }
    }
}
