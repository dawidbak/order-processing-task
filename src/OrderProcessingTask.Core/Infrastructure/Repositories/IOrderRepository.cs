using OrderProcessingTask.Core.Domain;

namespace OrderProcessingTask.Core.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<string> GetOrderAsync(int orderId);
    Task AddOrderAsync(Order order);
}