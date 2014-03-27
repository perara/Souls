using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Souls.Model.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Tool.hbm2ddl;
using Souls.Model;
using FluentNHibernate.Automapping;
using FluentNHibernate.Data;

namespace SoulsModel
{
    public class NHibernateHelper
    {
        private static NHibernateHelper instance = new NHibernateHelper();

        private ISessionFactory _sessionFactory;

        private NHibernateHelper()
        {

            _sessionFactory = CreateSessionFactory();

        }


        private ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
            .Database(MySQLConfiguration.Standard
            .ConnectionString(@"Server=persoft.no;Port=6001;Database=souls;Uid=root;Pwd=Perpass1;"))
            .Mappings(m => m.FluentMappings

            // Add maps
            .AddFromAssemblyOf<Ability>()
            .AddFromAssemblyOf<Card>()
            .AddFromAssemblyOf<Game>()
            .AddFromAssemblyOf<GameLog>()
            .AddFromAssemblyOf<GameLogType>()
            .AddFromAssemblyOf<Player>()
            .AddFromAssemblyOf<PlayerLogin>()
            .AddFromAssemblyOf<PlayerType>()
            .AddFromAssemblyOf<Race>()
            )

            .BuildSessionFactory();

        }

        public static ISession OpenSession()
        {
            return instance._sessionFactory.OpenSession();
        }
    }
}
