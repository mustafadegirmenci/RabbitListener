using RabbitListener.Core.Services;

namespace RabbitListener.Worker;

public class Worker : BackgroundService
{    
    private const int DelayMilliseconds = 100;
    private readonly RabbitService _rabbitService;
    
    public Worker(RabbitService rabbitService)
    {
        _rabbitService = rabbitService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitService.Init();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _rabbitService.ExecuteAsync();
            await Task.Delay(DelayMilliseconds, stoppingToken);
        }
        
        _rabbitService.OnComplete();
    }
}
