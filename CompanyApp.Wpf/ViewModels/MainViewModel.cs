namespace CompanyApp.Wpf.ViewModels;

public class MainViewModel
{
    public MainViewModel(
        EmployeeListViewModel employees,
        CounterpartyListViewModel counterparties,
        OrderListViewModel orders)
    {
        Employees = employees;
        Counterparties = counterparties;
        Orders = orders;
    }

    public EmployeeListViewModel Employees { get; }

    public CounterpartyListViewModel Counterparties { get; }

    public OrderListViewModel Orders { get; }

    public async Task InitializeAsync()
    {
        await Employees.LoadCommand.ExecuteAsync(null);
        await Counterparties.LoadCommand.ExecuteAsync(null);
        await Orders.LoadCommand.ExecuteAsync(null);
    }
}
