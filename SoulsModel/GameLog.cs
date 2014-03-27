using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class GameLog {
        public virtual int id { get; set; }
        public virtual GameLogType gameLogType { get; set; }
        public virtual Game game { get; set; }
    }
}
