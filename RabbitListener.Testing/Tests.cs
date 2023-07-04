using RabbitListener.Core.Services;

namespace RabbitListener.Testing;

[TestFixture]
public class Tests
{
    private HttpService _httpService;
    private QueueListener _queueListener;
    private QueueManager _queueManager;
    
    [SetUp]
    public void Setup()
    {
        _httpService = new HttpService();
        _queueListener = new QueueListener();
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
    public async Task Test_Queue_Fetch_Read(string testMessage)
    {
        string? readMessage = null;
        try
        {
            await QueueManager.FetchMessage(testMessage);
            readMessage = await QueueManager.ReadMessageFromQueue();
        }
        catch (Exception)
        {
            Assert.Fail();
        }

        if (readMessage == null)
        {
            Assert.Fail();
        }
        else if (readMessage.Equals(testMessage))
        {
            Assert.Pass();
        }
    }
}
