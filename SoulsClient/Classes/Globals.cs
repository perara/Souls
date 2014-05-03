using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoulsClient.Classes
{
    public class Globals
    {
        private static Globals instance { get; set; }

        public Dictionary<int, string> races { get; set; }

        private Globals()
        {
            races = new Dictionary<int, string>();
            races.Add(1, "/Content/Images/Card/Texture/darkness.png");
            races.Add(2, "/Content/Images/Card/Texture/vampiric.png");
            races.Add(3, "/Content/Images/Card/Texture/lightbringer.png");
            races.Add(4, "/Content/Images/Card/Texture/ferocious.png");
        }

        public static Globals GetInstance()
        {
            if (instance == null)
            {
                instance = new Globals();
            }

            return instance;
        }

    }
}