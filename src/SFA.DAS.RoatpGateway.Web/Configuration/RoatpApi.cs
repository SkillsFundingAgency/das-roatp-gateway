using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SFA.DAS.RoatpGateway.Web.Configuration
{
    public class RoatpApi
    {
        public string ApiBaseUrl { get; set; }
        public string SubscriptionKey { get; set; }
    }
}
