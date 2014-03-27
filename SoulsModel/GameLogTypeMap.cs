using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class GameLogTypeMap : ClassMap<GameLogType> {
        
        public GameLogTypeMap() {
			Table("game_log_type");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			Map(x => x.title).Column("title");
			Map(x => x.description).Column("description");
        }
    }
}
