using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class EmployeeListViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private ObservableCollection<Employee> _items = [];

    [ObservableProperty]
    private Employee? _selectedItem;

    public EmployeeListViewModel(
        IEmployeeService employeeService,
        IDialogService dialogService,
        IMessageBoxService messageBoxService)
    {
        _employeeService = employeeService;
        _dialogService = dialogService;
        _messageBoxService = messageBoxService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var employees = await _employeeService.GetAllAsync();
        Items = new ObservableCollection<Employee>(employees);
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (_dialogService.ShowEmployeeDialog(EditMode.Add) == true)
        {
            await LoadAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task EditAsync()
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (_dialogService.ShowEmployeeDialog(EditMode.Edit, SelectedItem) == true)
        {
            await LoadAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void View()
    {
        if (SelectedItem is null)
        {
            return;
        }

        _dialogService.ShowEmployeeDialog(EditMode.View, SelectedItem);
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (!_messageBoxService.Confirm($"Удалить сотрудника «{SelectedItem.FullName}»?"))
        {
            return;
        }

        try
        {
            await _employeeService.DeleteAsync(SelectedItem.Id);
            await LoadAsync();
        }
        catch (ValidationException ex)
        {
            _messageBoxService.ShowError(ex.Message);
        }
    }

    private bool HasSelection() => SelectedItem is not null;

    partial void OnSelectedItemChanged(Employee? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}
