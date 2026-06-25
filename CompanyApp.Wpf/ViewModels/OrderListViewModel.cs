using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApp.Wpf.ViewModels;

public partial class OrderListViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private ObservableCollection<Order> _items = [];

    [ObservableProperty]
    private Order? _selectedItem;

    public IAsyncRelayCommand RefreshCommand { get; }

    public OrderListViewModel(
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
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            var items = await orderService.GetAllAsync();
            var selectedId = SelectedItem?.Id;

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }

            if (selectedId.HasValue)
            {
                SelectedItem = Items.FirstOrDefault(o => o.Id == selectedId.Value);
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
        if (_dialogService.ShowOrderDialog(EditMode.Add) == true)
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

        if (_dialogService.ShowOrderDialog(EditMode.Edit, SelectedItem) == true)
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

        _dialogService.ShowOrderDialog(EditMode.View, SelectedItem);
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
        {
            return;
        }

        if (!_messageBoxService.Confirm($"Удалить заказ №{SelectedItem.Id}?"))
        {
            return;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            await orderService.DeleteAsync(SelectedItem.Id);
            await RefreshAsync();
        }
        catch (ValidationException ex)
        {
            _messageBoxService.ShowError(ex.Message);
        }
    }

    private bool HasSelection() => SelectedItem is not null;

    partial void OnSelectedItemChanged(Order? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        ViewCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}
