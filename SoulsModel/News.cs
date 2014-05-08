using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class News {
        public virtual int id { get; set; }
        public virtual string title { get; set; }
        public virtual string text { get; set; }
        public virtual string author { get; set; }
        public virtual DateTime date { get; set; }
        public virtual int enabled { get; set; }
    }
}
