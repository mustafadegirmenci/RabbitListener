using Microsoft.Extensions.Logging;

namespace RabbitListener.Core.Services;

public class LoggerService
{
    private readonly ILogger<RabbitService> _logger;

    public LoggerService(ILogger<RabbitService> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string? message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }
    
    public void LogError(string? message, params object?[] args)
    {
        _logger.LogError(message, args);
    }
}
