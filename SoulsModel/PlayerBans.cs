using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class PlayerBans {
        public virtual int id { get; set; }
        public virtual Player player { get; set; }
        public virtual DateTime? until { get; set; }
    }
}
