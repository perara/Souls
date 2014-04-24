using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoulsModel;
using Souls.Model;
using NHibernate.Linq;

namespace SoulsClient.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stats(int id)
        {
            ViewBag.gameId = id;

            using (var session = NHibernateHelper.OpenSession())
            {
                Game game = session.Query<Game>()
                    .Where(x => x.id == id)

                    .Fetch(x => x.player1)
                    .ThenFetch(x => x.playerType)
                    .ThenFetch(x => x.race)

                    .Fetch(x => x.player2)
                    .ThenFetch(x => x.playerType)
                    .ThenFetch(x => x.race)

                    .FirstOrDefault();


                // If this game does not exist
                if (game == null)
                {
                    // Return error page
                }

                // Fetch GameLog
                List<GameLog> log = session.Query<GameLog>()
                    .Where(x => x.game == game)
                    .Fetch(x => x.gameLogType)
                    .ToList();

                List<Card> cards = session.Query<Card>()
                   .Fetch(x => x.race)
                   .ToList();

                ViewBag.cards = cards;
                ViewBag.log = log;
                ViewBag.game = game;





            }



            return View();
        }


        //
        // GET: /Game/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Game/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Game/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Game/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Game/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Game/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Game/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
