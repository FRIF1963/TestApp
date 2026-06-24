using CompanyApp.Application.Abstractions;
using CompanyApp.Application.Exceptions;
using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Employee>();
        return await Task.FromResult(repository.Query().OrderBy(e => e.FullName).ToList());
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Employee>();
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<int> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        Validate(employee);

        var repository = _unitOfWork.GetRepository<Employee>();
        await repository.AddAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return employee.Id;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        Validate(employee);

        var repository = _unitOfWork.GetRepository<Employee>();
        var existing = await repository.GetByIdAsync(employee.Id, cancellationToken)
            ?? throw new ValidationException("Сотрудник не найден.");

        existing.FullName = employee.FullName;
        existing.Position = employee.Position;
        existing.BirthDate = employee.BirthDate;

        await repository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employeeRepository = _unitOfWork.GetRepository<Employee>();
        var counterpartyRepository = _unitOfWork.GetRepository<Counterparty>();
        var orderRepository = _unitOfWork.GetRepository<Order>();

        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ValidationException("Сотрудник не найден.");

        var isCurator = counterpartyRepository.Query().Any(c => c.Curator.Id == id);
        if (isCurator)
        {
            throw new ValidationException("Нельзя удалить сотрудника: он указан как куратор контрагента.");
        }

        var hasOrders = orderRepository.Query().Any(o => o.Employee.Id == id);
        if (hasOrders)
        {
            throw new ValidationException("Нельзя удалить сотрудника: он указан в заказах.");
        }

        await employeeRepository.DeleteAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private static void Validate(Employee employee)
    {
        if (string.IsNullOrWhiteSpace(employee.FullName))
        {
            throw new ValidationException("ФИО обязательно для заполнения.");
        }

        if (employee.BirthDate.Date > DateTime.Today)
        {
            throw new ValidationException("Дата рождения не может быть в будущем.");
        }
    }
}
