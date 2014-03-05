using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Tools
{
    public class Pair<T>
    {
        public Pair()
        {
        }

        public Pair(T first, T second)
        {
            this.First = first;
            this.Second = second;
        }

        public T Find(T obj)
        {
            T retObj = default(T);
            if(this.First.Equals(obj))
            {
                retObj = this.First;
            }
            else if(this.Second.Equals(obj))
            {
                retObj = this.Second;
            }
            return retObj;
        }

        public T this[int index]
        {
            get
            {
                if (index == 1) return First;
                else if (index == 2) return Second;
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 1) First = value;
                else if (index == 2) Second = value;
                else throw new IndexOutOfRangeException();
            }
        }



        public T First { get; set; }
        public T Second { get; set; }
    };

   

}
