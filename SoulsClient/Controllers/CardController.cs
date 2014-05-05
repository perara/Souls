using Souls.Client.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Souls.Model.Helpers;
using NHibernate.Linq;
using Souls.Model;
using Newtonsoft.Json.Linq;

namespace SoulsClient.Controllers
{

    public class CardController : Controller
    {
        public JsonpResult CardTextures()
        {
            JsonpResult result = null;
            using(var session = NHibernateHelper.OpenSession())
            {
                var items = session.Query<Card>()
                    .Select(x => new {x.id , x.portrait}).ToList();

                result = new JsonpResult(new { data = items });


            }

            return result;
        }
	}
}