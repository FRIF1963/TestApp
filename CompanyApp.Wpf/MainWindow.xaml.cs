using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CompanyApp.Wpf.ViewModels;

namespace CompanyApp.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EmployeesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Employees.ViewCommand.Execute(null);
        }
    }

    private void CounterpartiesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Counterparties.ViewCommand.Execute(null);
        }
    }

    private void OrdersGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Orders.ViewCommand.Execute(null);
        }
    }
}
