using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System.Web.Helpers;
using SoulsModel;
using Souls.Model;
using NHibernate.Criterion;
using NHibernate;

namespace SoulsClient.Controllers
{
    public class PlayerController : BaseController
    {

        // GET: /Player/
        public ActionResult Index()
        {
            // If user is not logged in
            if (cSession.Current.hash.Equals(""))
            {
                // Redirect to login controller
                return RedirectToAction("Login");
            }
            else
            {
                // Redirect to Player home
                return RedirectToAction("Index");
            }


        }

        public JsonpResult Hash()
        {
            JsonpResult result = new JsonpResult(new { hash = cSession.Current.hash });
            return result;
        }

        // GET: /Player/Login/
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Player/Login/
        [HttpPost]
        public ActionResult Login(Souls.Model.Player player)
        {


            using (var session = NHibernateHelper.OpenSession())
            {





                Player playerRecord = session.CreateCriteria<Player>()
                    .Add(Restrictions.Eq("name", player.name))
                    .Add(Restrictions.Eq("password", Toolkit.sha256_hash(player.password)))
                    .UniqueResult<Player>();
                session.SaveOrUpdate(playerRecord);

                // Check if the player record exists
                if (playerRecord != null)
                {
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
                        cSession.Current.hash = newHash;
                        cSession.Current.playerId = playerRecord.id;

                        // Set auth
                        FormsAuthentication.SetAuthCookie(newHash, false);
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

        // GET: /Player/Details/5
        public ActionResult Details(int? id)
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
