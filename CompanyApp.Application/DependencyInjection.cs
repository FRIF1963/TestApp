using CompanyApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddTransient<ICounterpartyService, CounterpartyService>();
        services.AddTransient<IOrderService, OrderService>();

        return services;
    }
}
