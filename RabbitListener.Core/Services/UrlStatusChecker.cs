using RabbitListener.Core.Entities;

namespace RabbitListener.Core.Services;

public class UrlStatusChecker
{
    private readonly HttpService _httpService;

    public UrlStatusChecker(HttpService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<UrlStatus> CheckUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) {
            throw new ArgumentNullException(nameof(url));
        }

        var statusCode = await _httpService.GetUrlResponseStatusCodeAsync(url);

        return new UrlStatus(url, statusCode);
    }
}
