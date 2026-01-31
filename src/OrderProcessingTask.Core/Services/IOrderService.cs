using OrderProcessingTask.Core.Domain;

namespace OrderProcessingTask.Core.Services;

public interface IOrderService
{
    Task ProcessOrderAsync(int orderId);
    Task AddOrderAsync(Order order);
}