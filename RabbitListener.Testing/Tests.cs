using Microsoft.Extensions.Logging;
using Moq;
using RabbitListener.Core.Entities;
using RabbitListener.Core.Services;

namespace RabbitListener.Testing;

[TestFixture]
public class Tests
{
    private Mock<ILogger<RabbitService>> _logger;
    private HttpService _httpService;
    private LoggerService _loggerService;
    private QueueListener _queueListener;
    private RabbitService _rabbitService;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<RabbitService>>();
        _httpService = new HttpService();
        _queueListener = new QueueListener();
        _loggerService = new LoggerService(_logger.Object);

        _rabbitService = new RabbitService(_loggerService, _httpService);
    }

    [Test]
    public async Task Test_Url_Null()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("");
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.EmptyOrNullUrl));
    }
    
    [Test]
    public async Task Test_Url_Whitespace()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("   ");
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.EmptyOrNullUrl));
    }
    
    [TestCase("google")]
    public async Task Test_Url_Invalid(string urlToTest)
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(urlToTest);
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.InvalidUrl));
    }         

    [TestCase("https://www. google.com/")]
    public async Task Test_Url_Contains_Whitespace(string urlToTest)
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(urlToTest);
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.UrlWithWhitespace));
    }        
    
    [TestCase("https://www.google/")]
    public async Task Test_Url_Invalid_Request(string urlToTest)
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(urlToTest);
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.InvalidHttpRequest));
    }    
    
    [TestCase("https://www.google.com/")]
    [TestCase("https://www.youtube.com/")]
    public async Task Test_Url_Healthy(string urlToTest)
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(urlToTest);
        
        Assert.That(errorCode, Is.EqualTo(200));
    }    
    
    [TestCase("https://www.google.com/fsafsafsa")]
    public async Task Test_Url_Healthy_But_Inaccessible(string urlToTest)
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(urlToTest);
        
        Assert.That(errorCode, Is.EqualTo(404));
    }    
    
    [TestCase("urlsssss")]
    [TestCase("2")]
    [TestCase("URLS")]
    public void Test_Queue_Name_Invalid(string queueNameToTest)
    {
        var state = _queueListener.TryStartListening(queueNameToTest);
        
        Assert.IsFalse(state, $"Queue name does not match.\nGiven: {queueNameToTest}\nShould be: urls");
    }    
    
    [TestCase("urls")]
    public void Test_Queue_Name_Valid(string queueNameToTest)
    {
        var state = _queueListener.TryStartListening(queueNameToTest);
        
        Assert.IsTrue(state);
    }

    [TestCase("1")]
    public async Task Test_Queue_MessageMatches(string testMessage)
    {
        await QueueManager.FetchMessage(testMessage);
        string? readMessage = await QueueManager.ReadMessageFromQueue();
        
        Assert.Multiple(() =>
        {
            Assert.NotNull(readMessage);
            if (readMessage != null) Assert.That(testMessage, Is.EqualTo(readMessage));
        });
    }

    [Test]
    public async Task Test_RabbitService_Execute()
    {
        QueueManager.MessageQueue.Clear();
        QueueManager.MessageQueue.Enqueue("TEST");
        
        _rabbitService.Init();
        await _rabbitService.ExecuteAsync(loggingEnabled: false);
        _rabbitService.OnComplete();
        
        Assert.That(QueueManager.MessageQueue, Is.Empty);
    }

    [TestCase("example", 111)]
    [TestCase("example", HttpService.UrlResponseErrorCode.InvalidUrl)]
    [TestCase("example", HttpService.UrlResponseErrorCode.InvalidHttpRequest)]
    [TestCase("example", HttpService.UrlResponseErrorCode.EmptyOrNullUrl)]
    [TestCase("example", HttpService.UrlResponseErrorCode.UrlWithWhitespace)]
    public void Test_LoggerService_LogStatus(string testUrl, int testStatusCode)
    {
        var statusToLog = new UrlStatus(testUrl, testStatusCode);

        Assert.DoesNotThrow(() =>
        {
            _loggerService.LogStatusInfo(statusToLog);
        });
    }
    
    [TestCase("example")]
    public void Test_LoggerService_LogInfoAndWarning(string testMessage)
    {
        Assert.DoesNotThrow(() =>
        {
            _loggerService.LogInformation(testMessage);
            _loggerService.LogWarning(testMessage);
        });
    }
}
