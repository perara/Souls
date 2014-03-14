using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoulsClient.Model;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System.Web.Helpers;

namespace SoulsClient.Controllers
{
    public class PlayerController : BaseController
    {
        private soulsEntities db = new soulsEntities();


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
        public ActionResult Login(Model.db_Player player)
        {


            using (var db = new soulsEntities())
            {
                var p = db.db_Player.FirstOrDefault(u => u.name == player.name);

                bool isValid = false;
                // See if the player exists
                if (p == null)
                {
                    isValid = false;
                }
                else
                {
                    // Check if the password corresponds with the database
                    if (p.password == Toolkit.sha256_hash(player.password))
                    {
                        isValid = true;
                    }
                }

                // Check if there already exists a login record for the player (if so do update)
                var loginExists = db.db_Player_Hash.FirstOrDefault(u => u.fk_player_id == p.id);

                string playerIP = Request.ServerVariables["REMOTE_ADDR"];
                long currTimestamp = Toolkit.getTimestamp();


                // TODO this generates a SHA256 which is used to identify the user throughout the session
                string sessionHash = Toolkit.sha256_hash(currTimestamp + p.id + playerIP);

                // Was found?
                if (loginExists == null)
                {
                    // Login record not found, add new one
                    db_Player_Hash pHash = new db_Player_Hash();
                    pHash.fk_player_id = p.id;
                    pHash.hash = sessionHash;
                   
                    // Adds the new player hash
                    db.db_Player_Hash.Add(pHash);
                }
                else
                {
                    // Login record found, update existing
                    var existingLogin = db.db_Player_Hash.FirstOrDefault(x => x.fk_player_id == p.id);
                    existingLogin.hash = sessionHash;

                }
                // Saves changes done to the DB.
                db.SaveChanges();

                if (isValid)
                {
                    if (sessionHash == null)
                    {
                        throw new System.Exception("Session hash creation failed, Debug PlayerController :! ");
                    }

                    cSession.Current.hash = sessionHash;
                    cSession.Current.playerId = p.id;

                    FormsAuthentication.SetAuthCookie(sessionHash, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login details are wrong.");
                }
            }



            return View(player);
        }

        // GET: /Player/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db_Player player = db.db_Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
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
        public ActionResult Create([Bind(Include = "id,name,password,rank,timestamp")] db_Player player)
        {
            if (ModelState.IsValid)
            {
                db.db_Player.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: /Player/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db_Player player = db.db_Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,password,rank,timestamp")] db_Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        // GET: /Player/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db_Player player = db.db_Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: /Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db_Player player = db.db_Player.Find(id);
            db.db_Player.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
