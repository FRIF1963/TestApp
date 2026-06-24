using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default);

    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
