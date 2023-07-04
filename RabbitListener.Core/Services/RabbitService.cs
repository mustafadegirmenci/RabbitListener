using Microsoft.Extensions.Logging;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class RabbitService
{
    private readonly ILogger<RabbitService> _logger;
    private readonly LoggerService _loggerService;
    private readonly HttpService _httpService;

    public RabbitService(
        ILogger<RabbitService> logger,
        LoggerService loggerService,
        HttpService httpService)
    {
        _logger = logger;
        _loggerService = loggerService;
        _httpService = httpService;
    }

    public void Init()
    {
        var queueReceiver = new QueueListener();

        if (queueReceiver.TryStartListening("urls"))
        {
            _logger?.LogInformation("RabbitListener service started at: {time}", DateTimeOffset.Now);
        }
        else
        {
            _logger?.LogWarning("Couldn't start RabbitListener service.");
        }
    }

    public void OnComplete()
    {
        _logger?.LogInformation("RabbitListener service stopped at: {time}", DateTimeOffset.Now);
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