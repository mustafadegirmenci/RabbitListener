using System.Net;

namespace RabbitListener.Core.Entities;

public struct UrlStatus
{
    public string ServiceName { get; set; }
    public string Url { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
