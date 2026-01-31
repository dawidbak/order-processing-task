namespace OrderProcessingTask.Core.Infrastructure.Notification;

public interface INotificationService
{
    void Send(string message);
}