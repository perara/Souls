using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class AbilityMap : ClassMap<Ability> {
        
        public AbilityMap() {
			Table("ability");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			Map(x => x.name).Column("name").Not.Nullable();
			Map(x => x.parameter).Column("parameter");
        }
    }
}
