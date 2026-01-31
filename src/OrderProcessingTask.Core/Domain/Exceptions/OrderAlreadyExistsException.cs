namespace OrderProcessingTask.Core.Domain.Exceptions;

public class OrderAlreadyExistsException : Exception
{
    public int OrderId { get; }

    public OrderAlreadyExistsException(int orderId)
        : base($"Order with ID {orderId} already exists.")
    {
        OrderId = orderId;
    }
}