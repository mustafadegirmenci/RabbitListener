namespace RabbitListener.Core.Entities;

public class UrlStatus
{
    public string ServiceName { get; set; }
    public string Url { get; set; }
    public int StatusCode { get; set; }

    public UrlStatus(string url, int statusCode)
    {
        ServiceName = "RabbitListener";
        Url = url;
        StatusCode = statusCode;
    }
}
