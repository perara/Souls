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

namespace SoulsModel
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)

                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                .ConnectionString(@"Server=persoft.no;Port=6001;Database=souls;Uid=root;Pwd=Perpass1;") // Modify your ConnectionString
                .ShowSql()
                )
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<CardMap>())

                    .ExposeConfiguration(cfg => new SchemaExport(cfg)
                    .Create(true, false))
                    .BuildSessionFactory();
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
