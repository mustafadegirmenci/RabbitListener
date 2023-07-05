using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitListener.Core.Entities;

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
    
    public void LogWarning(string? message, params object?[] args)
    {
        _logger.LogWarning(message, args);
    }
    
    public void LogStatusInfo(UrlStatus status)
    {
        switch (status.StatusCode)
        {
            case (int)HttpService.UrlResponseErrorCode.InvalidUrl:
                _logger.LogWarning("Invalid url: {url}", status.Url);
                break;

            case (int)HttpService.UrlResponseErrorCode.InvalidHttpRequest:
                _logger.LogWarning("Invalid http request to: {url}", status.Url);
                break;

            case (int)HttpService.UrlResponseErrorCode.EmptyOrNullUrl:
                _logger.LogWarning("Url is empty.");
                break;

            case (int)HttpService.UrlResponseErrorCode.UrlWithWhitespace:
                _logger.LogWarning("Url ({url}) contains whitespaces.", status.Url);
                break;

            default:
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var statusAsJson = JsonSerializer.Serialize(status, options);

                _logger.LogInformation(statusAsJson);
                break;
        }
    }
}
