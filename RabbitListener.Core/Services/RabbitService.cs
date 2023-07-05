using System.Diagnostics;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class RabbitService
{
    private readonly LoggerService _loggerService;
    private readonly HttpService _httpService;
    private readonly ConsoleProgressBar _consoleProgressBar;

    public RabbitService(
        LoggerService loggerService,
        HttpService httpService,
        ConsoleProgressBar consoleProgressBar)
    {
        _loggerService = loggerService;
        _httpService = httpService;
        _consoleProgressBar = consoleProgressBar;
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
    
    public async Task ExecuteAsync(bool loggingEnabled = false)
    {
        var message = await QueueManager.ReadMessageFromQueue();
        if (string.IsNullOrEmpty(message)) return;

        const int requestCount = 10000;
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var statusCodes = await GetUrlResponseStatusCodesAsync(message, requestCount, _consoleProgressBar);
        stopwatch.Stop();

        _loggerService.LogInformation($"All the responses received.\nElapsed time: {stopwatch.Elapsed.ToString()}\nRequests per second: {requestCount/stopwatch.Elapsed.TotalSeconds}");
        
        foreach (var statusCode in statusCodes)
        {
            var urlStatus = new UrlStatus(message, statusCode);

            if (loggingEnabled) _loggerService.LogStatusInfo(urlStatus);
        }
    }

    private async Task<int[]> GetUrlResponseStatusCodesAsync(string url, int requestCount, IProgress<float> progress)
    {
        var tasks = new List<Task<int>>();

        _consoleProgressBar.Init(requestCount);
        
        for (var i = 0; i < requestCount; i++)
        {
            var task = _httpService.GetUrlResponseStatusCodeAsync(url);
            task.GetAwaiter().OnCompleted(() =>
            {
                progress.Report(0);
            });
            tasks.Add(task);
        }

        return await Task.WhenAll(tasks);
    }
}