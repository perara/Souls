using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Souls.Model;

namespace Souls.Client.Classes
{
    public class cSession
    {
        // private constructor
        private cSession()
        {
            player = null;
            login = null;
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

        public bool isLogin()
        {

            return (cSession.Current.player == null) ? false : true;
        }

        public Player player { get; set; }
        public PlayerLogin login { get; set; }
    }
}