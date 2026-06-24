using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public interface ICounterpartyService
{
    Task<IReadOnlyList<Counterparty>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Counterparty?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Counterparty counterparty, CancellationToken cancellationToken = default);

    Task UpdateAsync(Counterparty counterparty, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
