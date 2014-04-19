using Souls.Model;
using SoulsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoulsClient.Controllers
{

    [AllowAnonymousAttribute]
    public class ShopController : Controller
    {
        //
        // GET: /Shop/
        public ActionResult Index()
        {
            ViewBag.cards = GetCards();
            return View();
        }

        public List<Card> GetCards()
        {

            List<Card> cards = null;
            using (var session = NHibernateHelper.OpenSession())
            {
                cards = session.Query<Card>().ToList();
            }
            return cards;
        }
    }
}