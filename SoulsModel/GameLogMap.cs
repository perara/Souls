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
			Map(x => x.obj1id).Column("obj1id").Not.Nullable();
			Map(x => x.obj2id).Column("obj2id").Not.Nullable();
			Map(x => x.obj1type).Column("obj1Type").Not.Nullable();
			Map(x => x.obj2type).Column("obj2Type").Not.Nullable();
        }
    }
}
