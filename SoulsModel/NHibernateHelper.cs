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
using System.Reflection;

namespace Souls.Model.Helpers
{
    public class NHibernateHelper
    {
        private static NHibernateHelper instance;

        private ISessionFactory _sessionFactory;

        private NHibernateHelper()
        {

            _sessionFactory = CreateSessionFactory();

        }


        private ISessionFactory CreateSessionFactory()
        {

            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                .ConnectionString(@"Server=db.hgsd.persoft.lan;Port=3306;Database=souls;Uid=souls;Pwd=souls;"))
                .Mappings(
                    m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())

                )
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();


            /* return Fluently.Configure()
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

             .BuildSessionFactory();*/

        }

        public static ISession OpenSession()
        {
            instance = new NHibernateHelper();
            return instance._sessionFactory.OpenSession();
        }
    }
}
