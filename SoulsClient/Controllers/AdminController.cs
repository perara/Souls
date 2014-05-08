using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Souls.Model.Helpers;
using NHibernate.Linq;
using Souls.Model;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Drawing;
using Souls.Client.Classes;
using System.Diagnostics;

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
        public ActionResult CardEditor(int? id)
        {
            Card c = null;

            using (var session = NHibernateHelper.OpenSession())
            {
                List<Ability> abList = session.Query<Ability>().ToList();
                ViewBag.abilities = abList;

                List<Race> raceList = session.Query<Race>().ToList();
                ViewBag.races = raceList;

                List<Card> cardList = session.Query<Card>()
                    .Fetch(x => x.race)
                    .Fetch(x => x.ability)
                    .ToList();
                ViewBag.cards = cardList;

                c = session.Query<Card>().Where(x => x.id == id).FirstOrDefault();

                if (c == null)
                {
                    c = new Card();
                }

            }

            if (id != null)
            {
                ViewBag.action = "Update";
            }
            else
            {
                ViewBag.action = "Create";
            }

            if (c.portrait == null)
            {
                ViewBag.portrait = null;
            }
            else
            {
                ViewBag.portrait = c.portrait;
            }

            return View(c);
        }

        public ActionResult NewsEditor(int? id)
        {
            News n = null;

            using (var session = NHibernateHelper.OpenSession())
            {
                n = session.Query<News>().Where(x => x.id == id).FirstOrDefault();

                List<News> newsList = session.Query<News>().ToList();
                ViewBag.news = newsList;
            }

            if (id != null) ViewBag.action = "Update";
            else ViewBag.action = "Create";

            if (n == null)
            {
                n = new News();
            }

            if (n.enabled == 1) ViewBag.statusOption = "Disable";
            else ViewBag.statusOption = "Enable";

            return View(n);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewsEditor(News n)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    n.title = (String.IsNullOrEmpty(n.title)) ? "No Title" : n.title;
                    n.text = (String.IsNullOrEmpty(n.text)) ? "No Text" : n.text;
                    n.enabled = 1;
                    n.author = cSession.Current.player.name;
                    n.date = DateTime.Now;

                    session.SaveOrUpdate(n);

                    transaction.Commit();
                }
            }

            return RedirectToAction("NewsEditor");
        }

        public ActionResult NewsToggle(int id)
        {

            News n = null;

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    n = session.Query<News>().Where(x => x.id == id).FirstOrDefault();
                    if (n == null)
                    {
                        n = new News();
                    }

                    if (n.enabled == 1)
                    {
                        n.enabled = 0;
                        ViewBag.toggle = "Disabled";
                    }
                    else
                    {
                        n.enabled = 1;
                        ViewBag.toggle = "Enabled";
                    }
                    session.Update(n);
                    transaction.Commit();
                }
            }
            return RedirectToAction("NewsEditor");
        }

        public ActionResult NewsDelete(int id)
        {
            News n = null;

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    n = session.Query<News>().Where(x => x.id == id).FirstOrDefault();

                    session.Delete(n);
                    transaction.Commit();
                }
            }
            return RedirectToAction("NewsEditor");
        }

        [HttpPost]
        public ActionResult CardEditor(Card c, FormCollection form)
        {


            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    c.race = session.Query<Race>().Where(x => x.cardUrl == form["c_race"]).FirstOrDefault();
                    c.ability = session.Query<Ability>().Where(x => x.name == form["c_ability"]).FirstOrDefault();

                    session.SaveOrUpdate(c);

                    var source = Server.MapPath("~" + c.portrait);
                    var virtPath = "/Content/Images/Card/Portraits/" + c.id + Path.GetExtension(c.portrait);
                    var target = Server.MapPath("~" + virtPath);
                    System.IO.File.Copy(source, target, true);

                    c.portrait = virtPath;
                    session.Update(c);

                    transaction.Commit();

                }
            }


            return RedirectToAction("CardEditor");
        }

        [HttpPost]
        public JsonResult Upload()
        {
            if (Request.Files.Count == 0) return Json(new { data = "x" });


            var file = Request.Files[0];
            string retData = "NULL";
            var extension = Path.GetExtension(file.FileName);


            // extract only the fielname
            var fileName = Path.GetFileName(file.FileName);
            // store the file inside ~/App_Data/uploads folder

            string date = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var path = Path.Combine(Server.MapPath("~/Content/Uploads"), date + "_" + fileName);
            file.SaveAs(path);
            if (!IsValidImage(path))
            {
                System.IO.File.Delete(path);
                return Json(new { data = "xx" });
            }


            retData = "/Content/Uploads/" + date + "_" + fileName;


            // }

            return Json(new { data = retData }); // Return the path
        }

        bool IsValidImage(string filename)
        {
            try
            {
                Image newImage = Image.FromFile(filename);
            }
            catch (OutOfMemoryException ex)
            {
                // Image.FromFile will throw this if file is invalid.
                // Don't ask me why.
                return false;
            }
            return true;
        }
    }
}
