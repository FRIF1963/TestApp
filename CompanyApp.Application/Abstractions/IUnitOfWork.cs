namespace CompanyApp.Application.Abstractions;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRepository<T> GetRepository<T>() where T : class;

    Task CommitAsync(CancellationToken cancellationToken = default);
}
