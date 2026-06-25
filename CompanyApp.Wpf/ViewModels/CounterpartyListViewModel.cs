using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApp.Wpf.ViewModels;

public partial class CounterpartyListViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private ObservableCollection<Counterparty> _items = [];

    [ObservableProperty]
    private Counterparty? _selectedItem;

    public IAsyncRelayCommand RefreshCommand { get; }

    public CounterpartyListViewModel(
        IServiceScopeFactory scopeFactory,
        IDialogService dialogService,
        IMessageBoxService messageBoxService)
    {
        _scopeFactory = scopeFactory;
        _dialogService = dialogService;
        _messageBoxService = messageBoxService;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, AsyncRelayCommandOptions.AllowConcurrentExecutions);
    }

    public async Task RefreshAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var counterpartyService = scope.ServiceProvider.GetRequiredService<ICounterpartyService>();
            var items = await counterpartyService.GetAllAsync();
            var selectedId = SelectedItem?.Id;

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }

            if (selectedId.HasValue)
            {
                SelectedItem = Items.FirstOrDefault(c => c.Id == selectedId.Value);
            }
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (_dialogService.ShowCounterpartyDialog(EditMode.Add) == true)
        {
            await RefreshAsync();
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
            await RefreshAsync();
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
            using var scope = _scopeFactory.CreateScope();
            var counterpartyService = scope.ServiceProvider.GetRequiredService<ICounterpartyService>();
            await counterpartyService.DeleteAsync(SelectedItem.Id);
            await RefreshAsync();
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
