using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class GameLogType {
        public GameLogType() { }
        public virtual int id { get; set; }
        public virtual string title { get; set; }
        public virtual string description { get; set; }
    }
}
