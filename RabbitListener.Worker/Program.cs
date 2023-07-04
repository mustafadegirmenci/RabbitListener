using RabbitListener.Core.Services;

namespace RabbitListener.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<RabbitService, RabbitService>();
                
                services.AddSingleton<QueueManager, QueueManager>();
                services.AddSingleton<QueueListener, QueueListener>();
                
                services.AddTransient<HttpService, HttpService>();
                services.AddTransient<StatusLoggerService, StatusLoggerService>();
        
                services.AddHostedService<Worker>();
            })
            .Build();

        host.Run();
    }
}
