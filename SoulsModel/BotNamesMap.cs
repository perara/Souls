using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class BotNamesMap : ClassMap<BotNames> {
        
        public BotNamesMap() {
			Table("bot_names");
			LazyLoad();
            Id(x => x.id).GeneratedBy.Identity().Column("id");
			Map(x => x.name).Column("name").Not.Nullable();
        }
    }
}
