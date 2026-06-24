using System.Windows;
using CompanyApp.Application;
using CompanyApp.Infrastructure;
using CompanyApp.Wpf.Services;
using CompanyApp.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CompanyApp.Wpf;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure(context.Configuration);
                services.AddApplication();

                services.AddSingleton<IMessageBoxService, MessageBoxService>();
                services.AddSingleton<IDialogService, DialogService>();

                services.AddSingleton<EmployeeListViewModel>();
                services.AddSingleton<CounterpartyListViewModel>();
                services.AddSingleton<OrderListViewModel>();
                services.AddSingleton<MainViewModel>();

                services.AddTransient<EmployeeEditViewModel>();
                services.AddTransient<CounterpartyEditViewModel>();
                services.AddTransient<OrderEditViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainViewModel;
        await mainViewModel.InitializeAsync();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
