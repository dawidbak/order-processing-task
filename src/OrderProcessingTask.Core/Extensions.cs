using Microsoft.Extensions.DependencyInjection;
using OrderProcessingTask.Core.Infrastructure.Logging;
using OrderProcessingTask.Core.Infrastructure.Persistence;
using OrderProcessingTask.Core.Infrastructure.Repositories;
using OrderProcessingTask.Core.Services;
using OrderProcessingTask.Core.Services.Validators;

namespace OrderProcessingTask.Core;

public static class Extensions
{
    public static void MapCore(this IServiceCollection services)
    {
        services.MapApplication();
        services.MapInfrastructure();
    }
    
    private static void MapInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryOrderStore>();
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
    
    private static void MapApplication(this IServiceCollection services)
    {
        services.AddSingleton<IOrderValidator, OrderValidator>();
        services.AddScoped<IOrderService, OrderService>();
    }
}