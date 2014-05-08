using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class PlayerMap : ClassMap<Player> {
        
        public PlayerMap() {
			Table("player");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.playerType).Column("fk_type").Not.Nullable();
			Map(x => x.name).Column("name").Not.Nullable().Unique();
			Map(x => x.password).Column("password").Not.Nullable();
			Map(x => x.rank).Column("rank").Not.Nullable();
            Map(x => x.money).Column("money").Not.Nullable();
			Map(x => x.created).Column("created");
            References(x => x.playerPermission).Column("fk_permission");
        }
    }
}
