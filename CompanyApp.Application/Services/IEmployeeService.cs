using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
