using CompanyApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ICounterpartyService, CounterpartyService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
