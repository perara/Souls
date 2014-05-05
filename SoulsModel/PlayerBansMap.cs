using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class PlayerBansMap : ClassMap<PlayerBans> {
        
        public PlayerBansMap() {
			Table("player_bans");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.player).Column("fk_user");
			Map(x => x.until).Column("until");
        }
    }
}
