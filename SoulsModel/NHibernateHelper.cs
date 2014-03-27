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
        private static ISessionFactory _sessionFactory;


        /*protected ISessionFactory CreateSessionFactory()
        {
            var connectionString = @"Server=persoft.no;Port=6001;Database=souls;Uid=root;Pwd=Perpass1;";

            var autoMap = AutoMap.AssemblyOf<Entity>()
                .Where(t => typeof(Entity).IsAssignableFrom(t));

            return Fluently.Configure()
                .Database(
                    MySQLConfiguration.Standard.ConnectionString(connectionString))
                .Mappings(m => m.AutoMappings.Add(autoMap))
                .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }

        protected virtual void TreatConfiguration(NHibernate.Cfg.Configuration configuration)
        {
            var update = new SchemaUpdate(configuration);
            update.Execute(false, true);
        }*/


        private static void CreateSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                     .Database(MySQLConfiguration.Standard
                     .ConnectionString(@"Server=persoft.no;Port=6001;Database=souls;Uid=root;Pwd=Perpass1;") // Modify your ConnectionString
                     //.ShowSql()
                     )
                     .Mappings(m => m.FluentMappings
                         .AddFromAssemblyOf<Card>())

                         .ExposeConfiguration(cfg => new SchemaExport(cfg)
                         .Create(true, false))
                         .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
            if (_sessionFactory == null)
                NHibernateHelper.CreateSessionFactory();

            return _sessionFactory.OpenSession();
        }
    }
}
