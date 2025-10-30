using Newtonsoft.Json;
using SFA.DAS.RoatpGateway.Web.Settings.Authentication;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public string SessionCachingDatabase { get; set; }

        [JsonRequired]
        public string DataProtectionKeysDatabase { get; set; }

        [JsonRequired]
        public AuthSettings StaffAuthentication { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication RoatpServiceApiAuthentication { get; set; }

        [JsonRequired]
        public string EsfaAdminServicesBaseUrl { get; set; }
        public bool UseDfeSignIn { get; set; }
        public string DfESignInServiceHelpUrl { get; set; }
    }
}
