using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;
using CompanyApp.Wpf.ViewModels;

namespace CompanyApp.Wpf.Factories;

public interface IEmployeeEditViewModelFactory
{
    EmployeeEditViewModel Create(EditMode mode, Employee? employee = null);
}

public class EmployeeEditViewModelFactory : IEmployeeEditViewModelFactory
{
    private readonly IEmployeeService _employeeService;
    private readonly IMessageBoxService _messageBoxService;

    public EmployeeEditViewModelFactory(
        IEmployeeService employeeService,
        IMessageBoxService messageBoxService)
    {
        _employeeService = employeeService;
        _messageBoxService = messageBoxService;
    }

    public EmployeeEditViewModel Create(EditMode mode, Employee? employee = null)
        => new(_employeeService, _messageBoxService, mode, employee);
}

public interface ICounterpartyEditViewModelFactory
{
    CounterpartyEditViewModel Create(EditMode mode, Counterparty? counterparty = null);
}

public class CounterpartyEditViewModelFactory : ICounterpartyEditViewModelFactory
{
    private readonly ICounterpartyService _counterpartyService;
    private readonly IEmployeeService _employeeService;
    private readonly IMessageBoxService _messageBoxService;

    public CounterpartyEditViewModelFactory(
        ICounterpartyService counterpartyService,
        IEmployeeService employeeService,
        IMessageBoxService messageBoxService)
    {
        _counterpartyService = counterpartyService;
        _employeeService = employeeService;
        _messageBoxService = messageBoxService;
    }

    public CounterpartyEditViewModel Create(EditMode mode, Counterparty? counterparty = null)
        => new(_counterpartyService, _employeeService, _messageBoxService, mode, counterparty);
}

public interface IOrderEditViewModelFactory
{
    OrderEditViewModel Create(EditMode mode, Order? order = null);
}

public class OrderEditViewModelFactory : IOrderEditViewModelFactory
{
    private readonly IOrderService _orderService;
    private readonly IEmployeeService _employeeService;
    private readonly ICounterpartyService _counterpartyService;
    private readonly IMessageBoxService _messageBoxService;

    public OrderEditViewModelFactory(
        IOrderService orderService,
        IEmployeeService employeeService,
        ICounterpartyService counterpartyService,
        IMessageBoxService messageBoxService)
    {
        _orderService = orderService;
        _employeeService = employeeService;
        _counterpartyService = counterpartyService;
        _messageBoxService = messageBoxService;
    }

    public OrderEditViewModel Create(EditMode mode, Order? order = null)
        => new(_orderService, _employeeService, _counterpartyService, _messageBoxService, mode, order);
}
