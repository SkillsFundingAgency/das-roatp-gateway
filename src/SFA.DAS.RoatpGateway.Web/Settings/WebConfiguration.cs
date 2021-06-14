using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public AuthSettings StaffAuthentication { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        [JsonRequired]
        public ClientApiAuthentication RoatpRegisterApiAuthentication { get; set; }

        [JsonRequired]
        public string EsfaAdminServicesBaseUrl { get; set; }
    }
}
