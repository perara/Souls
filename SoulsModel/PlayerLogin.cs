using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class PlayerLogin {
        public virtual int id { get; set; }
        public virtual Player player { get; set; }
        public virtual string hash { get; set; }
        public virtual DateTime? timestamp { get; set; }
    }
}
