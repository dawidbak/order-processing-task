using OrderProcessingTask.Core.Infrastructure.Logging;

namespace OrderProcessingTask.Core.Common.Options;

public class LoggingOptions
{
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
}