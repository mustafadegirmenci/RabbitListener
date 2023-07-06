namespace RabbitListener.Core.Entities;

public struct HttpResponse
{
    public string Url;
    public HttpResponseMessage? Response;
    public Exception? Exception;
}
