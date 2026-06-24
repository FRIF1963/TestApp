using System.Windows;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.ViewModels;
using CompanyApp.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApp.Wpf.Services;

public interface IDialogService
{
    bool? ShowEmployeeDialog(EditMode mode, Employee? employee = null);

    bool? ShowCounterpartyDialog(EditMode mode, Counterparty? counterparty = null);

    bool? ShowOrderDialog(EditMode mode, Order? order = null);
}

public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool? ShowEmployeeDialog(EditMode mode, Employee? employee = null)
    {
        var viewModel = ActivatorUtilities.CreateInstance<EmployeeEditViewModel>(
            _serviceProvider, new object?[] { mode, employee });
        return ShowDialog<EmployeeEditWindow>(viewModel);
    }

    public bool? ShowCounterpartyDialog(EditMode mode, Counterparty? counterparty = null)
    {
        var viewModel = ActivatorUtilities.CreateInstance<CounterpartyEditViewModel>(
            _serviceProvider, new object?[] { mode, counterparty });
        return ShowDialog<CounterpartyEditWindow>(viewModel);
    }

    public bool? ShowOrderDialog(EditMode mode, Order? order = null)
    {
        var viewModel = ActivatorUtilities.CreateInstance<OrderEditViewModel>(
            _serviceProvider, new object?[] { mode, order });
        return ShowDialog<OrderEditWindow>(viewModel);
    }

    private static bool? ShowDialog<TWindow>(object viewModel) where TWindow : Window, new()
    {
        var window = new TWindow
        {
            DataContext = viewModel,
            Owner = System.Windows.Application.Current.MainWindow
        };

        return window.ShowDialog();
    }
}
