using System;
using System.Text;
using System.Collections.Generic;
using FluentNHibernate.Data;


namespace Souls.Model {

    public class Card
    {
        public virtual int id { get; set; }
        public virtual Ability ability { get; set; }
        public virtual Race race { get; set; }
        public virtual string name { get; set; }
        public virtual int attack { get; set; }
        public virtual int health { get; set; }
        public virtual int armor { get; set; }
        public virtual int cost { get; set; }
        public virtual int vendor_price { get; set; }
        public virtual int level { get; set; }
        public virtual string portrait { get; set; }
    }
}
