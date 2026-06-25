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
        await Employees.RefreshAsync();
        await Counterparties.RefreshAsync();
        await Orders.RefreshAsync();
    }
}
