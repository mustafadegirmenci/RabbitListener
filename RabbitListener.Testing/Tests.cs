using Microsoft.Extensions.Logging;
using Moq;
using RabbitListener.Core.Entities;
using RabbitListener.Core.Services;

namespace RabbitListener.Testing;

[TestFixture]
public class Tests
{
    private Mock<ILogger<RabbitService>> _logger;
    private Mock<ConsoleProgressBar> _consoleProgressBar;
    private HttpService _httpService;
    private LoggerService _loggerService;
    private QueueListener _queueListener;
    private RabbitService _rabbitService;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<RabbitService>>();
        _consoleProgressBar = new Mock<ConsoleProgressBar>();
        _httpService = new HttpService();
        _queueListener = new QueueListener();
        _loggerService = new LoggerService(_logger.Object);

        _rabbitService = new RabbitService(_loggerService, _httpService, _consoleProgressBar.Object, _queueListener);
        
        _rabbitService.Start();
    }

    [TestCase(" ")]
    [TestCase(null)]
    [TestCase("aaaaaa")]
    [TestCase("google.com")]
    public void Test_Url_Invalid(string urlToTest)
    {
        Assert.DoesNotThrow(() =>
        {
            _queueListener.OnMessageReceived?.Invoke(urlToTest);
        });
    }
    
    [TestCase("https://www.google.com/")]
    [TestCase("https://www.youtube.com/")]
    public void Test_Url_Valid(string urlToTest)
    {
        Assert.DoesNotThrow(() =>
        {
            _queueListener.OnMessageReceived?.Invoke(urlToTest);
        });
    }

    [TestCase("urlsssss")]
    [TestCase("2")]
    [TestCase("URLS")]
    public void Test_Queue_Name_Invalid(string queueNameToTest)
    {
        Assert.Throws<Exception>(() =>
        {
            _queueListener.StartListening(queueNameToTest);
        });
    }
    
    [TestCase("urls")]
    public void Test_Queue_Name_Valid(string queueNameToTest)
    {
        Assert.DoesNotThrow(() =>
        {
            _queueListener.StartListening(queueNameToTest);
        });
    }
    
    [TestCase("example")]
    public void Test_LoggerService_LogInfoAndWarning(string testMessage)
    {
        Assert.DoesNotThrow(() =>
        {
            _loggerService.LogInformation(testMessage);
            _loggerService.LogError(testMessage);
        });
    }
}
