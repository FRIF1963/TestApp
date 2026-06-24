using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApp.Application.Exceptions;
using CompanyApp.Application.Services;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Services;

namespace CompanyApp.Wpf.ViewModels;

public partial class OrderListViewModel : ObservableObject
{
    private readonly IOrderService _orderService;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty]
    private ObservableCollection<Order> _items = [];

    [ObservableProperty]
    private Order? _selectedItem;

    public OrderListViewModel(
        IOrderService orderService,
        IDialogService dialogService,
        IMessageBoxService messageBoxService)
    {
        _orderService = orderService;
        _dialogService = dialogService;
        _messageBoxService = messageBoxService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var items = await _orderService.GetAllAsync();
        Items = new ObservableCollection<Order>(items);
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (_dialogService.ShowOrderDialog(EditMode.Add) == true)
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

        if (_dialogService.ShowOrderDialog(EditMode.Edit, SelectedItem) == true)
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
            await _orderService.DeleteAsync(SelectedItem.Id);
            await LoadAsync();
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
