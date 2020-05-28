using System;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure
{
    public class ExternalApiException : Exception
    {
        public ExternalApiException() : base()
        {

        }

        public ExternalApiException(string message) : base(message)
        {

        }

        public ExternalApiException(string message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
