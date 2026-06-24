using CompanyApp.Infrastructure.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace CompanyApp.Infrastructure.Persistence;

public static class NHibernateHelper
{
    public static ISessionFactory BuildSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MySQLConfiguration.Standard
                .ConnectionString(connectionString)
                .Driver<NHibernate.Driver.MySqlDataDriver>()
                .Dialect<NHibernate.Dialect.MySQL5Dialect>())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<EmployeeMap>())
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration configuration)
    {
        var update = new SchemaUpdate(configuration);
        update.Execute(false, true);
    }
}
