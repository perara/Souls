using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System.Web.Helpers;
using Souls.Model;
using NHibernate.Criterion;
using NHibernate;
using NHibernate.Linq;
using Souls.Client.Classes;
using System.ComponentModel.DataAnnotations;
using Souls.Model.Helpers;
using SoulsClient.Classes;

namespace SoulsClient.Controllers
{

    public class PlayerController : BaseController
    {

        // GET: /Player/
        public ActionResult Index()
        {
            return View("Index");
        }

        public JsonpResult Hash()
        {
            JsonpResult result = new JsonpResult(new { hash = cSession.Current.login.hash });
            return result;
        }

        public JsonpResult GetPlayer()
        {

            Souls.Model.Player p;
            using (var session = NHibernateHelper.OpenSession())
            {
                p = session.Query<Souls.Model.Player>()
                    .Where(x => x.id == cSession.Current.player.id)
                    .Fetch(x => x.playerType)
                    .ThenFetch(x => x.race)
                    .SingleOrDefault();
            }


            return new JsonpResult(
                new
                {
                    name = p.name,
                    money = p.money,
                    rank = p.rank,
                    type = p.playerType.name,
                    health = p.playerType.health,
                    bonusmana = p.playerType.mana,
                    race = p.playerType.race.name
                }
            );
        }

        // GET: /Player/Login/
        [AllowAnonymousAttribute]
        public ActionResult Login()
        {
            if (cSession.Current.isLogin()) return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Player/Login/
        [HttpPost]
        [AllowAnonymousAttribute]

        public ActionResult Login(Souls.Model.Player player)
        {

            if (player.name == null)
            {
                ModelState.AddModelError("", "Username is empty!");
                return View(player);

            }
            else if (player.password == null)
            {
                ModelState.AddModelError("", "Password is empty!");
                return View(player);
            }


            using (var session = NHibernateHelper.OpenSession())
            {

                Player playerRecord = session.Query<Player>()
                    .Where(x => x.name == player.name)
                    .Where(x => x.password == Toolkit.sha256_hash(player.password))
                    .Fetch(x => x.playerType)
                    .ThenFetch(x => x.race)
                    .Fetch(x => x.playerType)
                    .ThenFetch(x => x.ability)
                    .Fetch(x => x.playerPermission)
                    .SingleOrDefault();

                // Check if the player record exists
                if (playerRecord != null)
                {
                    session.SaveOrUpdate(playerRecord);

                    using (ITransaction transaction = session.BeginTransaction())
                    {

                        // Try to get Login Record (If exists)
                        PlayerLogin loginRecord = session.CreateCriteria<PlayerLogin>()
                            .Add(Restrictions.Eq("player", playerRecord))
                            .UniqueResult<PlayerLogin>();

                        // If it does not exist
                        if (loginRecord == null)
                        {
                            loginRecord = new PlayerLogin();
                            loginRecord.player = playerRecord;
                        }


                        string pIp = Request.ServerVariables["REMOTE_ADDR"];
                        long timestamp = Toolkit.getTimestamp();
                        string newHash = Toolkit.sha256_hash(timestamp + playerRecord.id + pIp); //TODO 

                        loginRecord.hash = newHash;
                        loginRecord.timestamp = timestamp;

                        // Save or update :D
                        session.SaveOrUpdate(loginRecord);

                        // Commit it
                        transaction.Commit();

                        // Set session
                        player.password = null; // For security :)
                        cSession.Current.player = playerRecord;
                        cSession.Current.login = loginRecord;

                        var authTicket = new FormsAuthenticationTicket(1, newHash, DateTime.Now, DateTime.Now.AddHours(2),  /*Login Expiring*/true,  /*Remember Me*/"", /*roles */ "/");

                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                        Response.Cookies.Add(cookie);

                        return RedirectToAction("Index", "Home");


                    }
                }
                else
                {
                    ModelState.AddModelError("", "User does not exist!");
                }

            }

            return View(player);
        }

        // GET /Player/Register
        [AllowAnonymousAttribute]
        public ActionResult Register()
        {
            RegisterData();
            ViewBag.ValidationLog = ModelState;
            return View();
        }

        public void RegisterData()
        {
            Dictionary<int, PlayerClass> playerTypes = new Dictionary<int, PlayerClass>();
            using (var session = NHibernateHelper.OpenSession())
            {

                /// STEP 2
                //////////////////////////////////////////////////////////////////////////
                /// Fetch and create a dictionary with all races
                //////////////////////////////////////////////////////////////////////////
                var playerRaces = session.Query<Race>().ToList();
                foreach (var race in playerRaces)
                {
                    PlayerClass pClass = new PlayerClass(race);
                    playerTypes.Add(race.id, pClass);
                }

                //////////////////////////////////////////////////////////////////////////
                /// Fetch and add types to races
                //////////////////////////////////////////////////////////////////////////
                var types = session.Query<PlayerType>()
                    .Fetch(x => x.race)
                    .ToList();

                foreach (var type in types)
                {
                    playerTypes[type.race.id].AddPlayerType(type);
                }

                ///STEP3
                /// Fetch starter cards
                var cards = session.Query<Card>()
                    .Fetch(x => x.race)
                    .Fetch(x => x.ability)
                    .Where(x => x.level == 1).ToList();
                ViewBag.starterCards = cards;

                ViewBag.races = Globals.GetInstance().races;

            }

            ViewBag.types = playerTypes;
        }


        [HttpPost]
        [AllowAnonymousAttribute]
        public ActionResult Register(FormCollection formCollection)
        {
            bool success = true;

            string username = formCollection["username"];
            string password = formCollection["password"];
            string confirm_password = formCollection["confirm-password"];
            string email = formCollection["email"];
            string cards = formCollection["cards"];
            string type = formCollection["playerType"];

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(confirm_password) || String.IsNullOrEmpty(email) || cards == null || type == null)
            {
                if (String.IsNullOrEmpty(username)) ModelState.AddModelError("input-username", "You must input a username");
                if (String.IsNullOrEmpty(password)) ModelState.AddModelError("input-password", "You must input a password");
                if (String.IsNullOrEmpty(confirm_password)) ModelState.AddModelError("input-confirm-password", "You must input a confirmation password");
                if (String.IsNullOrEmpty(email)) ModelState.AddModelError("input-email", "You must input an email");
                if (cards == null) ModelState.AddModelError("input-cards", "You must select cards");
                if (type == null) ModelState.AddModelError("input-race", "You must select a race");
                success = false;
                RegisterData();
                ViewBag.ValidationLog = ModelState;
                return View();
            }

            // Check that passwords match
            if (password != confirm_password)
            {
                success = false;
                ModelState.AddModelError("password-match", "Password does not match!");
            }

            // Check minimum size
            if (password.Count() < 6)
            {
                success = false;
                ModelState.AddModelError("password-length", "Password must be 6 characters or longer!");
            }

            // Check Email
            if (!email.Contains("@"))
            {
                success = false;
                ModelState.AddModelError("email", "Email format is not correct!");
            }

            if (cards.Split(',').Count() != 5)
            {
                success = false;
                ModelState.AddModelError("cards", "You must select 5 cards!");
            }


            if (success)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    if (session.Query<Player>().Where(x => x.name == username).FirstOrDefault() == null)
                    {

                        Player p = new Player();
                        p.money = 0;
                        p.name = username;
                        p.password = Toolkit.sha256_hash(password);
                        p.rank = 1;
                        p.created = DateTime.Now;
                        p.playerType = session.Query<PlayerType>().Where(x => x.id == int.Parse(type)).FirstOrDefault();
                        session.Save(p);

                        foreach (var index in cards.Split(','))
                        {
                            var card = session.Query<Card>().Where(x => x.id == int.Parse(index)).FirstOrDefault();
                            PlayerCards pCards = new PlayerCards();
                            pCards.card = card;
                            pCards.player = p;
                            pCards.obtainedat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            session.Save(pCards);
                        }


                        using (var transaction = session.BeginTransaction())
                        {
                            transaction.Commit();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("username-exists", "The username already exists");
                        RegisterData();
                        ViewBag.ValidationLog = ModelState;
                        return View();
                    }
                }


                return RedirectToAction("Login");
            }

            RegisterData();
            ViewBag.ValidationLog = ModelState;
            return View();
        }





        public ActionResult Logout()
        {
            cSession.Current.player = null;
            cSession.Current.login = null;

            return RedirectToAction("Index", "Home");
        }


        // GET: /Player/Details/5
        public ActionResult Details(int? id)
        {

            return View();

        }

        // GET: /Player/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,password,rank,timestamp")] Player player)
        {
            if (ModelState.IsValid)
            {
                // db.db_Player.Add(player);
                // db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
            //return View(player);
        }

        // GET: /Player/Edit/5
        public ActionResult Edit(int? id)
        {
            /* if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             db_Player player = db.db_Player.Find(id);
             if (player == null)
             {
                 return HttpNotFound();
             }*/
            return View();
            // return View(player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,password,rank,timestamp")] Player player)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(player).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        // GET: /Player/Delete/5
        public ActionResult Delete(int? id)
        {
            /*  if (id == null)
              {
                  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
              }
              db_Player player = db.db_Player.Find(id);
              if (player == null)
              {
                  return HttpNotFound();
              }*/
            return View();
            // return View(player);
        }

        // POST: /Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*  db_Player player = db.db_Player.Find(id);
              db.db_Player.Remove(player);
              db.SaveChanges();*/
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {

            /*  if (disposing)
              {
                  db.Dispose();
              }
              base.Dispose(disposing);*/
        }

    }
}
