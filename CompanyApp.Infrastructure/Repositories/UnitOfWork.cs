using CompanyApp.Application.Abstractions;
using NHibernate;

namespace CompanyApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ISession _session;
    private readonly Dictionary<Type, object> _repositories = new();
    private ITransaction? _transaction;

    public UnitOfWork(ISessionFactory sessionFactory)
    {
        _session = sessionFactory.OpenSession();
        _transaction = _session.BeginTransaction();
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repository))
        {
            return (IRepository<T>)repository;
        }

        var newRepository = new Repository<T>(_session);
        _repositories[type] = newRepository;
        return newRepository;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.CommitAsync(cancellationToken);
        _transaction.Dispose();
        _transaction = _session.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _session.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
