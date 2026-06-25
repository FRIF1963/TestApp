using CompanyApp.Application.Abstractions;
using NHibernate;

namespace CompanyApp.Infrastructure.Repositories;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly ISessionFactory _sessionFactory;

    public UnitOfWorkFactory(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public IUnitOfWork Create() => new UnitOfWork(_sessionFactory);
}
