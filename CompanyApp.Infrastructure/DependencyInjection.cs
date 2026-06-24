using CompanyApp.Application.Abstractions;
using CompanyApp.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace CompanyApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Строка подключения 'Default' не найдена.");

        services.AddSingleton<ISessionFactory>(_ => NHibernateHelper.BuildSessionFactory(connectionString));
        services.AddTransient<IUnitOfWork, Repositories.UnitOfWork>();

        return services;
    }
}
