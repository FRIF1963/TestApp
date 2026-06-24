using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class CounterpartyListViewModel : ObservableObject
{
    private readonly ICounterpartyService _counterpartyService;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private ObservableCollection<Counterparty> _items = [];

    [ObservableProperty]
    private Counterparty? _selectedItem;

    public CounterpartyListViewModel(
        ICounterpartyService counterpartyService,
        IDialogService dialogService,
        IMessageBoxService messageBoxService)
    {
        _counterpartyService = counterpartyService;
        _dialogService = dialogService;
        _messageBoxService = messageBoxService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var items = await _counterpartyService.GetAllAsync();
        Items = new ObservableCollection<Counterparty>(items);
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (_dialogService.ShowCounterpartyDialog(EditMode.Add) == true)
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

        if (_dialogService.ShowCounterpartyDialog(EditMode.Edit, SelectedItem) == true)
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

        _dialogService.ShowCounterpartyDialog(EditMode.View, SelectedItem);
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (!_messageBoxService.Confirm($"Удалить контрагента «{SelectedItem.Name}»?"))
        {
            return;
        }

        try
        {
            await _counterpartyService.DeleteAsync(SelectedItem.Id);
            await LoadAsync();
        }
        catch (ValidationException ex)
        {
            _messageBoxService.ShowError(ex.Message);
        }
    }

    private bool HasSelection() => SelectedItem is not null;

    partial void OnSelectedItemChanged(Counterparty? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}
