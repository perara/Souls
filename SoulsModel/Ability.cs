using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class Ability {
        public Ability() { }
        public virtual int id { get; set; }
        public virtual string name { get; set; }
        public virtual string parameter { get; set; }
    }
}
