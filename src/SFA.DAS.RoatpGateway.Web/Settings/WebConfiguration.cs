using Newtonsoft.Json;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public AuthSettings StaffAuthentication { get; set; }

        [JsonRequired]
        public ClientApiAuthentication ApplyApiAuthentication { get; set; }
    }
}
