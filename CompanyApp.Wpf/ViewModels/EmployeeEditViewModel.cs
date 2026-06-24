using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Domain.Enums;
using CompanyApp.Wpf.Helpers;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class EmployeeEditViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private EnumItem<EmployeePosition>? _selectedPosition;

    [ObservableProperty]
    private DateTime? _birthDate = DateTime.Today.AddYears(-25);

    public EditMode Mode { get; }

    public int Id { get; private set; }

    public IReadOnlyList<EnumItem<EmployeePosition>> Positions { get; } = EnumHelper.GetItems<EmployeePosition>();

    public string Title => Mode switch
    {
        EditMode.Add => "Добавление сотрудника",
        EditMode.Edit => "Редактирование сотрудника",
        _ => "Просмотр сотрудника"
    };

    public bool IsReadOnly => Mode == EditMode.View;

    public bool CanSave => !IsReadOnly;

    public event Action<bool?>? CloseRequested;

    public EmployeeEditViewModel(
        IEmployeeService employeeService,
        IMessageBoxService messageBoxService,
        EditMode mode,
        Employee? employee = null)
    {
        _employeeService = employeeService;
        _messageBoxService = messageBoxService;
        Mode = mode;

        if (employee is not null)
        {
            Id = employee.Id;
            FullName = employee.FullName;
            SelectedPosition = Positions.FirstOrDefault(p => p.Value == employee.Position);
            BirthDate = employee.BirthDate;
        }
        else
        {
            SelectedPosition = Positions.FirstOrDefault();
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (SelectedPosition is null || BirthDate is null)
        {
            _messageBoxService.ShowError("Заполните все обязательные поля.");
            return;
        }

        var employee = new Employee
        {
            Id = Id,
            FullName = FullName.Trim(),
            Position = SelectedPosition.Value,
            BirthDate = BirthDate.Value
        };

        try
        {
            if (Mode == EditMode.Add)
            {
                await _employeeService.CreateAsync(employee);
            }
            else
            {
                await _employeeService.UpdateAsync(employee);
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
