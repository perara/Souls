using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class GameLogMap : ClassMap<GameLog> {
        
        public GameLogMap() {
			Table("game_log");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			References(x => x.gameLogType).Column("fk_log_type");
			References(x => x.game).Column("fk_game");
        }
    }
}
