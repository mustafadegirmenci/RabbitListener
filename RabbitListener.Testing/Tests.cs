using RabbitListener.Core.Services;

namespace RabbitListener.Testing;

[TestFixture]
public class Tests
{
    private HttpService _httpService;
    private QueueListener _queueListener;
    
    [SetUp]
    public void Setup()
    {
        _httpService = new HttpService();
        _queueListener = new QueueListener();
    }

    [Test]
    public async Task Test_Empty_Url()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync(" ");
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.EmptyOrNullUrl));
    }
    
    [Test]
    public async Task Test_Invalid_Url()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("dsafasfa");
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.InvalidUrl));
    }        
    
    [Test]
    public async Task Test_Invalid_Request()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("https://www.google/");
        
        Assert.That(errorCode, Is.EqualTo((int)HttpService.UrlResponseErrorCode.InvalidUrl));
    }    
    
    [Test]
    public async Task Test_Healthy_Url()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("https://www.google.com/");
        
        Assert.That(errorCode, Is.EqualTo(200));
    }    
    
    [Test]
    public async Task Test_Healthy_But_Inaccessible_Url()
    {
        var errorCode = await _httpService.GetUrlResponseStatusCodeAsync("https://www.google.com/fsafsafsa");
        
        Assert.That(errorCode, Is.EqualTo(404));
    }    
    
    [Test]
    public void Test_Invalid_Queue_Name()
    {
        var nameToTest = "urlsssss";
        var state = _queueListener.TryStartListening(nameToTest);
        
        Assert.IsFalse(state, $"Queue name does not match.\nGiven: {nameToTest}\nShould be: urls");
    }    
    
    [Test]
    public void Test_Valid_Queue_Name()
    {
        var state = _queueListener.TryStartListening("urls");
        
        Assert.IsTrue(state);
    }
}
