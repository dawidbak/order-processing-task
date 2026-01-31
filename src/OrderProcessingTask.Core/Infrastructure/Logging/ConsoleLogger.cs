namespace OrderProcessingTask.Core.Infrastructure.Logging;

public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        WriteLog("INFO", message);
    }

    public void LogError(string message, Exception ex)
    {
        WriteLog("ERROR", $"{message} - Exception: {ex.Message}\n{ex.StackTrace}");
    }
    
    
    private static void WriteLog(string level, string message)
    {
        var timestamp = TimeProvider.System.GetUtcNow().DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Console.WriteLine($"[{timestamp}] [{level}] {message}");
    }
}