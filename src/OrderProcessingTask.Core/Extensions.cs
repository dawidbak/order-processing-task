using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingTask.Core.Common.Options;
using OrderProcessingTask.Core.Infrastructure.Logging;
using OrderProcessingTask.Core.Infrastructure.Notification;
using OrderProcessingTask.Core.Infrastructure.Persistence;
using OrderProcessingTask.Core.Infrastructure.Repositories;
using OrderProcessingTask.Core.Services;
using OrderProcessingTask.Core.Services.Validators;

namespace OrderProcessingTask.Core;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public void MapCore(IConfiguration configuration)
        {
            services.MapApplication();
            services.MapInfrastructure(configuration);
        }

        private void MapInfrastructure(IConfiguration configuration)
        {
            services.AddSingleton<InMemoryOrderStore>();
            services.Configure<LoggingOptions>(configuration.GetSection("Logging"));
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        private void MapApplication()
        {
            services.AddSingleton<IOrderValidator, OrderValidator>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}