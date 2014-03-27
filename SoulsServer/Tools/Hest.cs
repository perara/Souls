using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souls.Server.Tools
{
    public class Hest
    {
        private static Hest instance;
        private Dictionary<int, string> dic = new Dictionary<int, string>();

        private Hest()
        {
            Construct();
        }

        public void Construct()
        {
            dic.Add(1, "Hello my name is {0} and i spoke {1} for poop in pants and {2}");
        }


        public String Get(int responseId, dynamic[] args)
        {
            String val;
            dic.TryGetValue(responseId, out val);
            if (val == null)
            {
                Console.WriteLine("Response id: " + responseId + "does not exist");
            }


            return String.Format(val, args);
        }


        public static Hest getInstance()
        {
            if (instance == null)
            {
                instance = new Hest();
            }

            return instance;
        }

    }
}

