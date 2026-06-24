using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class CounterpartyEditViewModel : ObservableObject
{
    private readonly ICounterpartyService _counterpartyService;
    private readonly IEmployeeService _employeeService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _inn = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = [];

    [ObservableProperty]
    private Employee? _selectedCurator;

    public EditMode Mode { get; }

    public int Id { get; private set; }

    private int? _initialCuratorId;

    public string Title => Mode switch
    {
        EditMode.Add => "Добавление контрагента",
        EditMode.Edit => "Редактирование контрагента",
        _ => "Просмотр контрагента"
    };

    public bool IsReadOnly => Mode == EditMode.View;

    public bool CanSave => !IsReadOnly;

    public event Action<bool?>? CloseRequested;

    public CounterpartyEditViewModel(
        ICounterpartyService counterpartyService,
        IEmployeeService employeeService,
        IMessageBoxService messageBoxService,
        EditMode mode,
        Counterparty? counterparty = null)
    {
        _counterpartyService = counterpartyService;
        _employeeService = employeeService;
        _messageBoxService = messageBoxService;
        Mode = mode;

        if (counterparty is not null)
        {
            Id = counterparty.Id;
            Name = counterparty.Name;
            Inn = counterparty.Inn;
            _initialCuratorId = counterparty.Curator?.Id;
        }

        _ = LoadEmployeesAsync();
    }

    private async Task LoadEmployeesAsync()
    {
        var employees = await _employeeService.GetAllAsync();
        Employees = new ObservableCollection<Employee>(employees);

        if (_initialCuratorId.HasValue)
        {
            SelectedCurator = Employees.FirstOrDefault(e => e.Id == _initialCuratorId.Value);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (SelectedCurator is null)
        {
            _messageBoxService.ShowError("Выберите куратора.");
            return;
        }

        var counterparty = new Counterparty
        {
            Id = Id,
            Name = Name.Trim(),
            Inn = Inn.Trim(),
            Curator = SelectedCurator
        };

        try
        {
            if (Mode == EditMode.Add)
            {
                await _counterpartyService.CreateAsync(counterparty);
            }
            else
            {
                await _counterpartyService.UpdateAsync(counterparty);
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
