using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoulsClient.Controllers
{
    public class cSession
    {
        // private constructor
        private cSession()
        {
            playerId = -1;
        }

        // Gets the current session.
        public static cSession Current
        {
            get
            {
                cSession session =
                  (cSession)HttpContext.Current.Session["__cSession__"];
                if (session == null)
                {
                    session = new cSession();
                    HttpContext.Current.Session["__cSession__"] = session;
                }
                return session;
            }
        }


        public int playerId { get; set; }
        public string hash { get; set; }
    }
}