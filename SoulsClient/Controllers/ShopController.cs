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

    public class ShopController : Controller
    {
        //
        // GET: /Shop/
        public ActionResult Index()
        {

            Dictionary<int, string> races = new Dictionary<int, string>();
            races.Add(1, "/Content/Images/Card/Texture/darkness.png");
            races.Add(2, "/Content/Images/Card/Texture/vampiric.png");
            races.Add(3, "/Content/Images/Card/Texture/lightbringer.png");
            races.Add(4, "/Content/Images/Card/Texture/ferocious.png");
            
            ViewBag.races = races;
            ViewBag.cards = GetCards();
            return View();
        }

        public List<Card> GetCards()
        {

            List<Card> cards = null;
            using (var session = NHibernateHelper.OpenSession())
            {
                cards = session.Query<Card>()
                    .Fetch(x => x.ability)
                    .ToList();
            }
            return cards;
        }
    }
}