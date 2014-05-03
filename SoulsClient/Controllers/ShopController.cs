using Souls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Web;
using System.Web.Mvc;
using Souls.Model.Helpers;
using Souls.Client.Classes;
using SoulsClient.Classes;

namespace SoulsClient.Controllers
{

    public class ShopController : Controller
    {
        //
        // GET: /Shop/
        public ActionResult Index()
        {

            Dictionary<int, string> races = Globals.GetInstance().races;

            if ((ModelStateDictionary)TempData["ModelState"] == null)
            {
                TempData["ModelState"] = new ModelStateDictionary();
            }



            ViewBag.ValidationLog = TempData["ModelState"];
            ViewBag.races = races;
            ViewBag.cards = GetCards();
            return View();
        }

        // POST: /Shop/Buy
        public ActionResult Buy(string cardid)
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {


                    var cardToBuy = session.Query<Card>().Where(x => x.id == int.Parse(cardid)).FirstOrDefault();

                    var alreadyBought = session.Query<PlayerCards>()
                        .Where(x => x.card == cardToBuy && x.player == cSession.Current.player)
                        .Fetch(x => x.card)
                        .FirstOrDefault();

                    if ((cSession.Current.player.money - cardToBuy.vendor_price) >= 0 && alreadyBought == null)
                    {
                        cSession.Current.player.money -= cardToBuy.vendor_price;


                        PlayerCards newOwnCard = new PlayerCards()
                        {
                            card = cardToBuy,
                            obtainedat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            player = cSession.Current.player
                        };



                        session.Update(cSession.Current.player);
                        session.Save(newOwnCard);
                        transaction.Commit();
                    }
                    else // Not enough money or its already bought
                    {
                        if (alreadyBought != null)
                            ViewData.ModelState.AddModelError("Own", "You already own " + alreadyBought.card.name); // Already own
                        if (cSession.Current.player.money - cardToBuy.vendor_price < 0)
                            ViewData.ModelState.AddModelError("Money", "You cannot afford " + cardToBuy.name + "!"); // Already own
                    }


                }

            }


            TempData["ModelState"] = ViewData.ModelState;
            return RedirectToAction("Index");
        }

        public List<Card> GetCards()
        {

            List<Card> cards = null;
            List<Card> ownedCards = null;
            using (var session = NHibernateHelper.OpenSession())
            {
                cards = session.Query<Card>()
                    .Fetch(x => x.ability)
                    .ToList();

                ownedCards = session.Query<PlayerCards>()
                    .Where(x => x.player == cSession.Current.player)
                    .Select(x => x.card)
                    .ToList();


                foreach (var ownedCard in ownedCards)
                {
                    if (cards.Exists(x => x.id == ownedCard.id))
                    {
                        cards.Where(x => x.id == ownedCard.id).FirstOrDefault().vendor_price = 1000000;
                    }
                }



            }
            return cards;
        }
    }
}