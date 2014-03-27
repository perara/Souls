using System;
using System.Text;
using System.Collections.Generic;


namespace Souls.Model
{

    public class PlayerType
    {
        public PlayerType() { }
        public virtual int id { get; set; }
        public virtual Ability ability { get; set; }
        public virtual Race race { get; set; }
        public virtual string name { get; set; }
        public virtual int attack { get; set; }
        public virtual int armor { get; set; }
        public virtual int health { get; set; }
        public virtual int mana { get; set; }
    }
}
