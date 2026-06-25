using CompanyApp.Application.Abstractions;
using CompanyApp.Application.Exceptions;
using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public OrderService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        var repository = unitOfWork.GetRepository<Order>();
        return await Task.FromResult(repository.Query().OrderByDescending(o => o.OrderDate).ToList());
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        var repository = unitOfWork.GetRepository<Order>();
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        await ValidateAsync(order, unitOfWork, cancellationToken);

        var repository = unitOfWork.GetRepository<Order>();
        await repository.AddAsync(order, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return order.Id;
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        await ValidateAsync(order, unitOfWork, cancellationToken);

        var repository = unitOfWork.GetRepository<Order>();
        var existing = await repository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new ValidationException("Заказ не найден.");

        existing.OrderDate = order.OrderDate;
        existing.Amount = order.Amount;
        existing.Employee = order.Employee;
        existing.Counterparty = order.Counterparty;

        await repository.UpdateAsync(existing, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();
        var repository = unitOfWork.GetRepository<Order>();
        var order = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ValidationException("Заказ не найден.");

        await repository.DeleteAsync(order, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private static async Task ValidateAsync(
        Order order,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
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

        var employeeRepository = unitOfWork.GetRepository<Employee>();
        var counterpartyRepository = unitOfWork.GetRepository<Counterparty>();

        order.Employee = await employeeRepository.GetByIdAsync(order.Employee.Id, cancellationToken)
            ?? throw new ValidationException("Выбранный сотрудник не найден.");

        order.Counterparty = await counterpartyRepository.GetByIdAsync(order.Counterparty.Id, cancellationToken)
            ?? throw new ValidationException("Выбранный контрагент не найден.");
    }
}
