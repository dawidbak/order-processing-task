using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingTask.Core;
using OrderProcessingTask.Core.Domain;
using OrderProcessingTask.Core.Services;

// Initialize DI container
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

services.MapCore(configuration);
var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Order Processing System");

using (var scope = serviceProvider.CreateScope())
{
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

    // Add orders for later processing
    await Task.WhenAll(
        orderService.AddOrderAsync(new Order { Id = 1, Description = "Laptop" }),
        orderService.AddOrderAsync(new Order { Id = 2, Description = "Phone" })
    );

    // Simulate multiple threads processing orders
    await Task.WhenAll(
        orderService.ProcessOrderAsync(1),
        orderService.ProcessOrderAsync(2),
        orderService.ProcessOrderAsync(-1));
}

Console.WriteLine("Processing complete.");