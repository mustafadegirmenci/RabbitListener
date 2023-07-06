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
                services.AddSingleton<QueueListener, QueueListener>();
                services.AddSingleton<HttpService, HttpService>();
                services.AddSingleton<ConsoleProgressBar, ConsoleProgressBar>();
                
                services.AddTransient<LoggerService, LoggerService>();
        
                services.AddHostedService<Worker>();
            })
            .Build();

        host.Run();
    }
}
