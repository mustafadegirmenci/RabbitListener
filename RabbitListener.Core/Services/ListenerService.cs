using Microsoft.Extensions.Logging;

namespace RabbitListener.Core.Services;

public class ListenerService
{
    private readonly ILogger<ListenerService> _logger;
    private readonly QueueReader _queueReader;
    private readonly UrlStatusChecker _urlStatusChecker;

    public ListenerService(
        ILogger<ListenerService> logger,
        QueueReader queueReader,
        UrlStatusChecker urlStatusChecker)
    {
        _logger = logger;
        _queueReader = queueReader;
        _urlStatusChecker = urlStatusChecker;
    }

    public void Init()
    {
        var queueReceiver = new QueueReceiver();
        queueReceiver.StartListening("urls");
    }
    
    public async Task ExecuteAsync()
    {
        var message = await _queueReader.GetMessageFromQueue();
        if (string.IsNullOrEmpty(message)) return;

        var urlStatus = await _urlStatusChecker.CheckUrlAsync(message);

        _logger.LogInformation(urlStatus.ToJson());
    }
}
