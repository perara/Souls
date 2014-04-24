using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class GameLog {
        public virtual int id { get; set; }
        public virtual GameLogType gameLogType { get; set; }
        public virtual Game game { get; set; }
        public virtual int obj1id { get; set; }
        public virtual int obj2id { get; set; }
        public virtual string obj1type { get; set; }
        public virtual string obj2type { get; set; }
    }
}
