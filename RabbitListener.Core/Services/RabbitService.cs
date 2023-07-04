using Microsoft.Extensions.Logging;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class RabbitService
{
    private readonly ILogger<RabbitService> _logger;
    private readonly StatusLoggerService _statusLoggerService;
    private readonly QueueManager _queueManager;
    private readonly HttpService _httpService;

    public RabbitService(
        ILogger<RabbitService> logger,
        StatusLoggerService statusLoggerService,
        QueueManager queueManager,
        HttpService httpService)
    {
        _logger = logger;
        _statusLoggerService = statusLoggerService;
        _queueManager = queueManager;
        _httpService = httpService;
    }

    public void Init()
    {
        var queueReceiver = new QueueListener();

        if (queueReceiver.TryStartListening("urls"))
        {
            _logger.LogInformation("RabbitListener service started at: {time}", DateTimeOffset.Now);
        }
        else
        {
            _logger.LogWarning("Couldn't start RabbitListener service.");
        }
    }

    public void OnComplete()
    {
        _logger.LogInformation("RabbitListener service stopped at: {time}", DateTimeOffset.Now);
    }

    public async Task ExecuteAsync()
    {
        var message = await _queueManager.ReadMessageFromQueue();
        if (string.IsNullOrEmpty(message)) return;

        var statusCode = await _httpService.GetUrlResponseStatusCodeAsync(message);
        var urlStatus = new UrlStatus(message, statusCode);

        _statusLoggerService.LogStatusInfo(urlStatus);
    }
}