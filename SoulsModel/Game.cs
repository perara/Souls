using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model {
    
    public class Game {
        public Game() { }
        public virtual int id { get; set; }
        public virtual Player player { get; set; }
    }
}
