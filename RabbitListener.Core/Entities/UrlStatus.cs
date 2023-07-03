using System.Text.Json;

namespace RabbitListener.Core.Entities;

public class UrlStatus
{
    public string ServiceName { get => "RabbitListener"; }
    public string Url { get; set; }
    public int StatusCode { get; set; }

    public UrlStatus(string url, int statusCode)
    {
        Url = url;
        StatusCode = statusCode;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(this, options);
    }
}
