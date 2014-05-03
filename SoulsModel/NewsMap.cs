using System; 
using System.Collections.Generic; 
using System.Text; 
using FluentNHibernate.Mapping;
using Souls.Model; 

namespace Souls.Model.Maps {
    
    
    public class NewsMap : ClassMap<News> {
        
        public NewsMap() {
			Table("news");
			LazyLoad();
			Id(x => x.id).GeneratedBy.Identity().Column("id");
			Map(x => x.title).Column("title").Not.Nullable();
			Map(x => x.text).Column("text").Not.Nullable();
			Map(x => x.author).Column("author").Not.Nullable();
			Map(x => x.date).Column("date").Not.Nullable();
        }
    }
}
