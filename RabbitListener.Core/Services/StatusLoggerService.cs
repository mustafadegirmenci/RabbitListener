using Microsoft.Extensions.Logging;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class StatusLoggerService
{
    private readonly ILogger<RabbitService> _logger;

    public StatusLoggerService(ILogger<RabbitService> logger)
    {
        _logger = logger;
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
                _logger.LogInformation(status.ToJson());
                break;
        }
    }
}
