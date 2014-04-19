using System;
using System.Text;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using SoulsModel;


namespace Souls.Model
{

    public class Player
    {
        public Player() { }
        public virtual int id { get; set; }
        public virtual PlayerType playerType { get; set; }
        public virtual string name { get; set; }
        public virtual string password { get; set; }
        public virtual int money { get; set; }
        public virtual int rank { get; set; }
        public virtual DateTime? created { get; set; }

        public virtual string GetHash()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                PlayerLogin pLogin = session.Query<PlayerLogin>()
                    .Where(x => x.player.id == this.id)
                    .SingleOrDefault();

                return pLogin.hash;
            }
        }
    }
}
