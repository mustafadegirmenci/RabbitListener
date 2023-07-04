namespace RabbitListener.Core.Services;

public class HttpService
{
    public enum UrlResponseErrorCode
    {
        InvalidHttpRequest,
        InvalidUrl,
        EmptyOrNullUrl
    }
    
    public async Task<int> GetUrlResponseStatusCodeAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(request);
            
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
        catch (ArgumentNullException)
        {
            return (int)UrlResponseErrorCode.EmptyOrNullUrl;
        }
    }
}
