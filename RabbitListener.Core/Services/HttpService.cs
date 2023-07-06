using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class HttpService
{
    private readonly HttpClient _client = new();

    public async Task<HttpResponse> SendRequestAsync(string url)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await _client.SendAsync(request);

            return new HttpResponse{ Url = url, Response = response, Exception = null };
        }
        catch (Exception e)
        {
            return new HttpResponse{ Url = url, Response = null, Exception = e };
        }
    }
}
