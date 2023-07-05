namespace RabbitListener.Core.Services;

public class HttpService
{
    private readonly HttpClient _client = new();
    
    public enum UrlResponseErrorCode
    {
        InvalidHttpRequest,
        InvalidUrl,
        EmptyOrNullUrl,
        UrlWithWhitespace
    }
    
    public async Task<int> GetUrlResponseStatusCodeAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return (int)UrlResponseErrorCode.EmptyOrNullUrl;
        }
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await _client.SendAsync(request);
            
            return (int)response.StatusCode;
        }
        catch (HttpRequestException)
        {
            return (int)UrlResponseErrorCode.InvalidHttpRequest;
        }
        catch (InvalidOperationException)
        {
            return (int)UrlResponseErrorCode.InvalidUrl;
        }
        catch (UriFormatException)
        {
            return (int)UrlResponseErrorCode.UrlWithWhitespace;
        }
    }
}
