using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Souls.Model.Helpers;
using NHibernate.Linq;
using Souls.Model;

namespace SoulsClient.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Players()
        {

            List<Player> players;
            using (var session = NHibernateHelper.OpenSession())
            {
                players = session.Query<Player>().ToList();
            }




            ViewBag.players = players;
            return View();
        }
        public ActionResult Games()
        {
            return View();
        }
        public ActionResult CardEditor()
        {
            return View();
        }
    }
}
