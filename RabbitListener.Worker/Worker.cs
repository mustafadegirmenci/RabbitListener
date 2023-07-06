using RabbitListener.Core.Services;

namespace RabbitListener.Worker;

public class Worker : BackgroundService
{    
    private readonly RabbitService _rabbitService;
    
    public Worker(RabbitService rabbitService) { _rabbitService = rabbitService;}

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitService.Start();
        return Task.CompletedTask;
    }
}
