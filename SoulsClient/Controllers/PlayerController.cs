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
