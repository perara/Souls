using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class PlayerCards {
        public virtual int id { get; set; }
        public virtual Player player { get; set; }
        public virtual Card card { get; set; }
        public virtual string obtainedat { get; set; }
    }
}
