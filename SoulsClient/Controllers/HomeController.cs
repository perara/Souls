using SoulsClient.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoulsClient.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            if (cSession.Current.player == null)
            {
                ViewBag.loggedIn = false;
            }
            else
            {
                ViewBag.loggedIn = true;
            }

     
            return View();
        }
    }
}
