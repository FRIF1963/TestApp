using CompanyApp.Application.Abstractions;
using NHibernate;
using NHibernate.Linq;

namespace CompanyApp.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ISession _session;

    public Repository(ISession session)
    {
        _session = session;
    }

    public IQueryable<T> Query() => _session.Query<T>();

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _session.GetAsync<T>(id, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _session.SaveAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _session.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _session.DeleteAsync(entity, cancellationToken);
    }
}
