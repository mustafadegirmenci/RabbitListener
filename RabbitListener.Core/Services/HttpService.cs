namespace RabbitListener.Core.Services;

public class HttpService
{
    public async Task<int> GetUrlResponseStatusCodeAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(request);
            
            return (int)response.StatusCode;
        }
        catch (HttpRequestException _)
        {
            return -1;
        }
        catch (InvalidOperationException _)
        {
            return -1;
        }
    }

}
