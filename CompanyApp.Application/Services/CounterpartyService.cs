using CompanyApp.Application.Abstractions;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Validation;
using CompanyApp.Domain.Entities;

namespace CompanyApp.Application.Services;

public class CounterpartyService : ICounterpartyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CounterpartyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Counterparty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Counterparty>();
        return await Task.FromResult(repository.Query().OrderBy(c => c.Name).ToList());
    }

    public async Task<Counterparty?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Counterparty>();
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<int> CreateAsync(Counterparty counterparty, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(counterparty, cancellationToken);

        var repository = _unitOfWork.GetRepository<Counterparty>();
        await repository.AddAsync(counterparty, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return counterparty.Id;
    }

    public async Task UpdateAsync(Counterparty counterparty, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(counterparty, cancellationToken);

        var repository = _unitOfWork.GetRepository<Counterparty>();
        var existing = await repository.GetByIdAsync(counterparty.Id, cancellationToken)
            ?? throw new ValidationException("Контрагент не найден.");

        existing.Name = counterparty.Name;
        existing.Inn = counterparty.Inn.Trim();
        existing.Curator = counterparty.Curator;

        await repository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var counterpartyRepository = _unitOfWork.GetRepository<Counterparty>();
        var orderRepository = _unitOfWork.GetRepository<Order>();

        var counterparty = await counterpartyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ValidationException("Контрагент не найден.");

        var hasOrders = orderRepository.Query().Any(o => o.Counterparty.Id == id);
        if (hasOrders)
        {
            throw new ValidationException("Нельзя удалить контрагента: существуют связанные заказы.");
        }

        await counterpartyRepository.DeleteAsync(counterparty, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ValidateAsync(Counterparty counterparty, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(counterparty.Name))
        {
            throw new ValidationException("Наименование обязательно для заполнения.");
        }

        InnValidator.Validate(counterparty.Inn);

        if (counterparty.Curator is null || counterparty.Curator.Id <= 0)
        {
            throw new ValidationException("Необходимо выбрать куратора.");
        }

        var employeeRepository = _unitOfWork.GetRepository<Employee>();
        var curator = await employeeRepository.GetByIdAsync(counterparty.Curator.Id, cancellationToken)
            ?? throw new ValidationException("Выбранный куратор не найден.");

        counterparty.Curator = curator;
    }
}
