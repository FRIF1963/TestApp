using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class OrderEditViewModel : ObservableObject
{
    private readonly IOrderService _orderService;
    private readonly IEmployeeService _employeeService;
    private readonly ICounterpartyService _counterpartyService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private DateTime? _orderDate = DateTime.Today;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = [];

    [ObservableProperty]
    private ObservableCollection<Counterparty> _counterparties = [];

    [ObservableProperty]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    private Counterparty? _selectedCounterparty;

    public EditMode Mode { get; }

    public int Id { get; private set; }

    private int? _initialEmployeeId;
    private int? _initialCounterpartyId;

    public string Title => Mode switch
    {
        EditMode.Add => "Добавление заказа",
        EditMode.Edit => "Редактирование заказа",
        _ => "Просмотр заказа"
    };

    public bool IsReadOnly => Mode == EditMode.View;

    public bool CanSave => !IsReadOnly;

    public event Action<bool?>? CloseRequested;

    public OrderEditViewModel(
        IOrderService orderService,
        IEmployeeService employeeService,
        ICounterpartyService counterpartyService,
        IMessageBoxService messageBoxService,
        EditMode mode,
        Order? order = null)
    {
        _orderService = orderService;
        _employeeService = employeeService;
        _counterpartyService = counterpartyService;
        _messageBoxService = messageBoxService;
        Mode = mode;

        if (order is not null)
        {
            Id = order.Id;
            OrderDate = order.OrderDate;
            Amount = order.Amount;
            _initialEmployeeId = order.Employee?.Id;
            _initialCounterpartyId = order.Counterparty?.Id;
        }

        _ = LoadLookupsAsync();
    }

    private async Task LoadLookupsAsync()
    {
        var employees = await _employeeService.GetAllAsync();
        var counterparties = await _counterpartyService.GetAllAsync();

        Employees = new ObservableCollection<Employee>(employees);
        Counterparties = new ObservableCollection<Counterparty>(counterparties);

        if (_initialEmployeeId.HasValue)
        {
            SelectedEmployee = Employees.FirstOrDefault(e => e.Id == _initialEmployeeId.Value);
        }

        if (_initialCounterpartyId.HasValue)
        {
            SelectedCounterparty = Counterparties.FirstOrDefault(c => c.Id == _initialCounterpartyId.Value);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (OrderDate is null || SelectedEmployee is null || SelectedCounterparty is null)
        {
            _messageBoxService.ShowError("Заполните все обязательные поля.");
            return;
        }

        var order = new Order
        {
            Id = Id,
            OrderDate = OrderDate.Value,
            Amount = Amount,
            Employee = SelectedEmployee,
            Counterparty = SelectedCounterparty
        };

        try
        {
            if (Mode == EditMode.Add)
            {
                await _orderService.CreateAsync(order);
            }
            else
            {
                await _orderService.UpdateAsync(order);
            }

            CloseRequested?.Invoke(true);
        }
        catch (ValidationException ex)
        {
            _messageBoxService.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(false);
    }
}
