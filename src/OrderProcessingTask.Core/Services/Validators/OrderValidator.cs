namespace OrderProcessingTask.Core.Services.Validators;

public interface IOrderValidator
{
    bool IsValid(int orderId);
}

public class OrderValidator : IOrderValidator
{
    public bool IsValid(int orderId)
    {
        return orderId > 0;
    }
}