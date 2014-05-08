using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Souls.Client.Classes;
using Souls.Model.Helpers;
using NHibernate.Linq;
using Souls.Model;

namespace SoulsClient.Classes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public int AccessLevel { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Not logged in at all :(
            if (cSession.Current == null)
                return false;

            bool auth = false;
            using (var session = NHibernateHelper.OpenSession())
            {

                var user = session.Query<Player>()
                    .Where(x => x.id == cSession.Current.player.id)
                    .FirstOrDefault();

                if (user.playerPermission.id >= AccessLevel)
                    auth = true;
            }

            return auth;
        }
    }

}