using OrderProcessingTask.Core.Domain;
using OrderProcessingTask.Core.Domain.Exceptions;
using OrderProcessingTask.Core.Infrastructure.Persistence;

namespace OrderProcessingTask.Core.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly InMemoryOrderStore _store;

    public OrderRepository(InMemoryOrderStore store)
    {
        _store = store;
    }

    public async Task<string> GetOrderAsync(int orderId)
    {
        if (!_store.Orders.TryGetValue(orderId, out var order))
            throw new KeyNotFoundException($"Order with id {orderId} not found!");
        
        await Task.Delay(100);
        return order.Description;
    }

    public async Task AddOrderAsync(Order order)
    {
        if (!_store.Orders.TryAdd(order.Id, order))
            throw new OrderAlreadyExistsException(order.Id);

        await Task.Delay(100);
    }
}