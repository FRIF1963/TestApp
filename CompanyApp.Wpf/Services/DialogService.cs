using System.Windows;
using CompanyApp.Domain.Entities;
using CompanyApp.Wpf.Factories;
using CompanyApp.Wpf.ViewModels;
using CompanyApp.Wpf.Views;

namespace CompanyApp.Wpf.Services;

public interface IDialogService
{
    bool? ShowEmployeeDialog(EditMode mode, Employee? employee = null);

    bool? ShowCounterpartyDialog(EditMode mode, Counterparty? counterparty = null);

    bool? ShowOrderDialog(EditMode mode, Order? order = null);
}

public class DialogService : IDialogService
{
    private readonly IEmployeeEditViewModelFactory _employeeEditViewModelFactory;
    private readonly ICounterpartyEditViewModelFactory _counterpartyEditViewModelFactory;
    private readonly IOrderEditViewModelFactory _orderEditViewModelFactory;

    public DialogService(
        IEmployeeEditViewModelFactory employeeEditViewModelFactory,
        ICounterpartyEditViewModelFactory counterpartyEditViewModelFactory,
        IOrderEditViewModelFactory orderEditViewModelFactory)
    {
        _employeeEditViewModelFactory = employeeEditViewModelFactory;
        _counterpartyEditViewModelFactory = counterpartyEditViewModelFactory;
        _orderEditViewModelFactory = orderEditViewModelFactory;
    }

    public bool? ShowEmployeeDialog(EditMode mode, Employee? employee = null)
    {
        var viewModel = _employeeEditViewModelFactory.Create(mode, employee);
        return ShowDialog<EmployeeEditWindow>(viewModel);
    }

    public bool? ShowCounterpartyDialog(EditMode mode, Counterparty? counterparty = null)
    {
        var viewModel = _counterpartyEditViewModelFactory.Create(mode, counterparty);
        return ShowDialog<CounterpartyEditWindow>(viewModel);
    }

    public bool? ShowOrderDialog(EditMode mode, Order? order = null)
    {
        var viewModel = _orderEditViewModelFactory.Create(mode, order);
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
