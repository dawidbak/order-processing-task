namespace OrderProcessingTask.Core.Infrastructure.Notification;

public class NotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"[NOTIFICATION]: {message}");
    }
}