using OrderProcessingTask.Core.Domain;
using OrderProcessingTask.Core.Domain.Exceptions;
using OrderProcessingTask.Core.Infrastructure.Logging;
using OrderProcessingTask.Core.Infrastructure.Repositories;
using OrderProcessingTask.Core.Services.Validators;

namespace OrderProcessingTask.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderValidator _orderValidator;
    private readonly ILogger _logger;
    private readonly IOrderRepository _repository;

    public OrderService(IOrderValidator orderValidator, ILogger logger, IOrderRepository repository)
    {
        _orderValidator = orderValidator;
        _logger = logger;
        _repository = repository;
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        _logger.LogInfo($"Starting processing for order id: {orderId}.");

        if (!_orderValidator.IsValid(orderId))
        {
            var exception = new ArgumentException("Id must be positive", nameof(orderId));
            _logger.LogError($"Order id: {orderId} is invalid.", exception);
        }

        try
        {
            await _repository.GetOrderAsync(orderId);
            _logger.LogInfo($"Order {orderId} processed successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError($"Order id: {orderId} not found.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while processing order {orderId}.", ex);
        }
    }

    public async Task AddOrderAsync(Order order)
    {
        try
        {
            await _repository.AddOrderAsync(order);
        }
        catch (OrderAlreadyExistsException ex)
        {
            _logger.LogError($"Failed to add new order with id {order.Id}", ex);
            throw;
        }
    }
}