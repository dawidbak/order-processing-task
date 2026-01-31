using System.Collections.Concurrent;
using OrderProcessingTask.Core.Domain;

namespace OrderProcessingTask.Core.Infrastructure.Persistence;

public class InMemoryOrderStore
{
    private readonly ConcurrentDictionary<int, Order> _orders = new();
    public ConcurrentDictionary<int, Order> Orders => _orders;
}