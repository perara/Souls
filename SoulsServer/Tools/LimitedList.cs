using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souls.Server.Tools
{
    public class LimitedList<T> : List<T>
    {
        public int MaxSize { get; set; }
        public LimitedList(int max)
        {
            this.MaxSize = max;
           
        }

        public LimitedList()
        {
            this.MaxSize = Int32.MaxValue;
        }



        public void TryAddRange(List<T> items)
        {
            if (items.Count() + this.Count() > MaxSize) throw new OverflowException();


            foreach (T item in items)
            {
                this.Add(item);
            }
         
        }

        public bool TryAdd(T item)
        {
            if (this.Count == MaxSize)
            {
                throw new OverflowException();
            }
            this.Add(item);
            return true;
        }


        public T RemoveAndReturn(int index)
        {
            T tmp = this[index];
            this.RemoveAt(index);

            return tmp;
        }
    }

}
