using CompanyApp.Application.Abstractions;
using CompanyApp.Application.Exceptions;
using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Order>();
        return await Task.FromResult(repository.Query().OrderByDescending(o => o.OrderDate).ToList());
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Order>();
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(order, cancellationToken);

        var repository = _unitOfWork.GetRepository<Order>();
        await repository.AddAsync(order, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return order.Id;
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(order, cancellationToken);

        var repository = _unitOfWork.GetRepository<Order>();
        var existing = await repository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new ValidationException("Заказ не найден.");

        existing.OrderDate = order.OrderDate;
        existing.Amount = order.Amount;
        existing.Employee = order.Employee;
        existing.Counterparty = order.Counterparty;

        await repository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Order>();
        var order = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ValidationException("Заказ не найден.");

        await repository.DeleteAsync(order, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ValidateAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.Amount <= 0)
        {
            throw new ValidationException("Сумма заказа должна быть больше нуля.");
        }

        if (order.Employee is null || order.Employee.Id <= 0)
        {
            throw new ValidationException("Необходимо выбрать сотрудника.");
        }

        if (order.Counterparty is null || order.Counterparty.Id <= 0)
        {
            throw new ValidationException("Необходимо выбрать контрагента.");
        }

        var employeeRepository = _unitOfWork.GetRepository<Employee>();
        var counterpartyRepository = _unitOfWork.GetRepository<Counterparty>();

        order.Employee = await employeeRepository.GetByIdAsync(order.Employee.Id, cancellationToken)
            ?? throw new ValidationException("Выбранный сотрудник не найден.");

        order.Counterparty = await counterpartyRepository.GetByIdAsync(order.Counterparty.Id, cancellationToken)
            ?? throw new ValidationException("Выбранный контрагент не найден.");
    }
}
