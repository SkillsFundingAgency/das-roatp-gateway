using System;
using System.Net;
using System.Text;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.Exceptions;

public class RoatpApiClientException : Exception
{
    public string HttpMethod { get; }
    public HttpStatusCode StatusCode { get; }
    public Uri RequestUri { get; }

    public RoatpApiClientException()
    {
    }

    public RoatpApiClientException(string httpMethod, HttpStatusCode statusCode, Uri requestUri, string message) : base(message)
    {
        HttpMethod = httpMethod;
        StatusCode = statusCode;
        RequestUri = requestUri;
    }

    public RoatpApiClientException(string httpMethod, HttpStatusCode statusCode, Uri requestUri, string message, Exception innerException) : base(message, innerException)
    {
        HttpMethod = httpMethod;
        StatusCode = statusCode;
        RequestUri = requestUri;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"HTTP {(int)StatusCode} || {HttpMethod}: {RequestUri}");

        if (!string.IsNullOrWhiteSpace(Message))
        {
            sb.AppendLine();
            sb.Append($"Message: {Message}");
        }

        if (InnerException != null)
        {
            sb.AppendLine();
            sb.Append($"InnerException: {InnerException.Message}");
        }

        return sb.ToString();
    }
}
