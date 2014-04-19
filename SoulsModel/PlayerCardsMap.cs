using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class PlayerCardsMap : ClassMap<PlayerCards> {
        
        public PlayerCardsMap() {
			Table("player_cards");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.player).Column("fk_player");
			References(x => x.card).Column("fk_card");
			Map(x => x.obtainedat).Column("obtainedAt").Not.Nullable();
        }
    }
}
