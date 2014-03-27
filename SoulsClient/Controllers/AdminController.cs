using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoulsClient.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Players()
        {
            int[] array = { 1, 2, 3, 4 };
            ViewBag.data = array;

            return View();
        }
        public ActionResult Games()
        {
            return View();
        }
        public ActionResult CardEditor()
        {
            return View();
        }
    }
}
