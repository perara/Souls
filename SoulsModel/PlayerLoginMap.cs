using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class PlayerLoginMap : ClassMap<PlayerLogin> {
        
        public PlayerLoginMap() {
			Table("player_login");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.player).Column("fk_player_id");
			Map(x => x.hash).Column("hash");
			Map(x => x.timestamp).Column("timestamp");
        }
    }
}
