using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class GameMap : ClassMap<Game> {
        
        public GameMap() {
			Table("game");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.player1).Column("fk_player1");
            References(x => x.player2).Column("fk_player2");
        }
    }
}
