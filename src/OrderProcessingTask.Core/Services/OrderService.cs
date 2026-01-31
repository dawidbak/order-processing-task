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

        if (!IsValidOrderId(orderId))
            return;

        try
        {
            await _repository.GetOrderAsync(orderId);
            _logger.LogInfo($"Order id: {orderId} processed successfully.");
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
            _logger.LogInfo($"Adding new order with id {order.Id}.");

            if (!IsValidOrderId(order.Id))
                return;

            await _repository.AddOrderAsync(order);

            _logger.LogInfo($"Order with id {order.Id} added successfully.");
        }
        catch (OrderAlreadyExistsException ex)
        {
            _logger.LogError($"Failed to add new order with id {order.Id}", ex);
            throw;
        }
    }

    private bool IsValidOrderId(int orderId)
    {
        if (_orderValidator.IsValid(orderId)) return true;
        var exception = new ArgumentException("Id must be positive", nameof(orderId));
        _logger.LogError($"Order id: {orderId} is invalid.", exception);
        return false;
    }
}