using System.Diagnostics;
using System.Text.Json;
using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class RabbitService
{
    private readonly LoggerService _loggerService;
    private readonly HttpService _httpService;
    private readonly ConsoleProgressBar _consoleProgressBar;
    private readonly QueueListener _queueListener;

    private readonly Queue<string> _fetchedMessageQueue = new();

    public RabbitService(
        LoggerService loggerService,
        HttpService httpService,
        ConsoleProgressBar consoleProgressBar, 
        QueueListener queueListener)
    {
        _loggerService = loggerService;
        _httpService = httpService;
        _consoleProgressBar = consoleProgressBar;
        _queueListener = queueListener;
    }

    public void Start()
    {
        _queueListener.StartListening();
        _queueListener.OnMessageReceived += OnQueueListenerOnMessageReceived;;
    }

    private async void OnQueueListenerOnMessageReceived(string message)
    {
        await FetchMessage(message);
    }

    private async Task FetchMessage(string message)
    {
        _fetchedMessageQueue.Enqueue(message);
        
        await HandleMessage();
    }

    private async Task HandleMessage()
    {
        if (_fetchedMessageQueue.Count == 0)
        {
            return;
        }

        var messageToProcess = _fetchedMessageQueue.Dequeue();
        
        SendRequestAndLogStatus(messageToProcess)
            .Wait();
        SendMultipleRequests(messageToProcess, 10000, true)
            .Wait();

        await HandleMessage();
    }

    private async Task SendMultipleRequests(string url, ushort requestCount, bool showProgressBar = false)
    {
        var tasks = new List<Task<HttpResponse>>();
        if (showProgressBar) _consoleProgressBar.Init(maxValue: requestCount);
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        _loggerService.LogInformation("Sending {numberOfMessages} HEAD requests to {url}.\n", requestCount, url);
        for (var i = 0; i < requestCount; i++)
        {
            var task = _httpService.SendRequestAsync(url);
            if (showProgressBar)
            {
                task.GetAwaiter().OnCompleted(() =>
                {
                    _consoleProgressBar.Report();
                });
            }
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        
        stopwatch.Stop();

        _loggerService.LogInformation(
            "All the responses received in {t} seconds with a speed of: {r} requests/sec", 
            stopwatch.Elapsed.TotalSeconds, requestCount/stopwatch.Elapsed.TotalSeconds);
    }    
    
    private async Task SendRequestAndLogStatus(string url)
    {
        var httpResponse = await _httpService.SendRequestAsync(url);
        LogResponse(httpResponse);
    }

    private void LogResponse(HttpResponse response)
    {
        var responseSuccess = response.Exception == null && response.Response != null;

        if (responseSuccess)
        {
            var urlStatus = new UrlStatus { ServiceName = "RabbitListener", Url = response.Url, StatusCode = response.Response.StatusCode};
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var statusAsJson = JsonSerializer.Serialize(urlStatus, options);

            _loggerService.LogInformation(statusAsJson);
        }
        else
        {
            switch (response.Exception)
            {
                case ArgumentNullException:
                    _loggerService.LogError("ArgumentNullException");
                    break;
                case InvalidOperationException:
                    if (string.IsNullOrWhiteSpace(response.Url))
                    {
                        _loggerService.LogError("Url is empty.");
                        break;
                    }
                    _loggerService.LogError("Url ({url}) is invalid.", response.Url);
                    break;                
                case HttpRequestException:
                    _loggerService.LogError("HttpRequestException");
                    break;
                default:
                    if (response.Url.Any(char.IsWhiteSpace))
                    {
                        _loggerService.LogError("URL ({url}) contains whitespaces.", response.Url);
                        break;
                    }
                    _loggerService.LogError("Another error.");
                    break;
            }
        }
    }
}
