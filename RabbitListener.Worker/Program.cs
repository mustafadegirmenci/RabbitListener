using RabbitListener.Core;
using RabbitListener.Core.Services;

namespace RabbitListener.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<ListenerService, ListenerService>();
                
                services.AddSingleton<QueueReader, QueueReader>();
                services.AddSingleton<QueueReceiver, QueueReceiver>();
                
                services.AddTransient<UrlStatusChecker, UrlStatusChecker>();
                services.AddTransient<HttpService, HttpService>();
        
                services.AddHostedService<Worker>();
            })
            .Build();

        host.Run();
    }
}
