using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.Exceptions
{
    [Serializable]
    public class RoatpApiClientException : ApplicationException
    {
        public string HttpMethod { get; }
        public HttpStatusCode StatusCode { get; }
        public Uri RequestUri { get; }

        public RoatpApiClientException()
        {
        }

        public RoatpApiClientException(HttpResponseMessage httpResponseMessage, string message)
            : this(httpResponseMessage?.RequestMessage?.Method.ToString(),
                  httpResponseMessage?.StatusCode ?? HttpStatusCode.BadRequest,
                  httpResponseMessage?.RequestMessage?.RequestUri,
                  message)
        {
        }

        public RoatpApiClientException(HttpResponseMessage httpResponseMessage, string message, Exception innerException)
            : this(httpResponseMessage?.RequestMessage?.Method.ToString(),
                  httpResponseMessage?.StatusCode ?? HttpStatusCode.BadRequest,
                  httpResponseMessage?.RequestMessage?.RequestUri,
                  message,
                  innerException)
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

        protected RoatpApiClientException(SerializationInfo info, StreamingContext context)
        {
            HttpMethod = info.GetValue("HttpMethod", typeof(string)) as string;
            StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
            RequestUri = info.GetValue("RequestUri", typeof(Uri)) as Uri;
        }

        [Obsolete("GetObjectData required")]  // Required
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.AddValue("HttpMethod", HttpMethod);
            info.AddValue("StatusCode", StatusCode);
            info.AddValue("RequestUri", RequestUri);
            base.GetObjectData(info, context);
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
}
