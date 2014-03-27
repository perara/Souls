using System;
using System.Text;
using System.Collections.Generic;
using NHibernate.Criterion;
using SoulsModel;


namespace Souls.Model {
    
    public class Player {
        public Player() { }
        public virtual int id { get; set; }
        public virtual PlayerType playerType { get; set; }
        public virtual string name { get; set; }
        public virtual string password { get; set; }
        public virtual int rank { get; set; }
        public virtual DateTime? created { get; set; }

        public virtual string GetHash()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                PlayerLogin hash = session.CreateCriteria<PlayerLogin>()
                    .Add(Restrictions.Eq(Projections.Property<PlayerLogin>(x => x.player), this.id))
                    .UniqueResult<PlayerLogin>();
                return hash.hash;
            }
        }
    }
}
