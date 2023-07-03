using RabbitListener.Core.Services;

namespace RabbitListener.Worker;

public class Worker : BackgroundService
{    
    private const int DelayMilliseconds = 1000;

    private readonly ILogger<Worker> _logger;
    private readonly ListenerService _listenerService;
    
    public Worker(ILogger<Worker> logger, ListenerService listenerService)
    {
        _logger = logger;
        _listenerService = listenerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitListener service started at: {time}", DateTimeOffset.Now);

        _listenerService.Init();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _listenerService.ExecuteAsync();
            await Task.Delay(DelayMilliseconds, stoppingToken);
        }
        
        _logger.LogInformation("RabbitListener service stopped at: {time}", DateTimeOffset.Now);
    }
}
