using System;
using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;
using Souls.Model;

namespace Souls.Model.Maps
{


    public class PlayerTypeMap : ClassMap<PlayerType>
    {

        public PlayerTypeMap()
        {
            Table("player_type");
            LazyLoad();
            Id(x => x.id).GeneratedBy.Identity().Column("id");
            References(x => x.ability).Column("fk_ability");
            References(x => x.race).Column("fk_race");
            Map(x => x.name).Column("name").Not.Nullable();
            Map(x => x.attack).Column("attack").Not.Nullable();
            Map(x => x.armor).Column("armor").Not.Nullable();
            Map(x => x.health).Column("health").Not.Nullable();
            Map(x => x.mana).Column("mana").Not.Nullable();
        }
    }
}
