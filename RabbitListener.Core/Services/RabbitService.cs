using Microsoft.Extensions.Logging;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class RabbitService
{
    private readonly LoggerService _loggerService;
    private readonly HttpService _httpService;

    public RabbitService(
        LoggerService loggerService,
        HttpService httpService)
    {
        _loggerService = loggerService;
        _httpService = httpService;
    }

    public void Init()
    {
        var queueReceiver = new QueueListener();

        if (queueReceiver.TryStartListening("urls"))
        {
            _loggerService.LogInformation("RabbitListener service started at: {time}", DateTimeOffset.Now);
        }
        else
        {
            _loggerService.LogWarning("Couldn't start RabbitListener service.");
        }
    }

    public void OnComplete()
    {
        _loggerService.LogInformation("RabbitListener service stopped at: {time}", DateTimeOffset.Now);
    }

    public async Task ExecuteAsync(bool loggingEnabled = true)
    {
        var message = await QueueManager.ReadMessageFromQueue();
        if (string.IsNullOrEmpty(message)) return;

        var statusCode = await _httpService.GetUrlResponseStatusCodeAsync(message);
        var urlStatus = new UrlStatus(message, statusCode);

        if (loggingEnabled) _loggerService.LogStatusInfo(urlStatus);
    }
}