using SFA.DAS.AdminService.Common.Settings;
using SFA.DAS.RoatpGateway.Web.Configuration;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        string SessionCachingDatabase { get; set; }

        string DataProtectionKeysDatabase { get; set; }

        AuthSettings StaffAuthentication { get; set; }

        ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        ClientApiAuthentication RoatpRegisterApiAuthentication { get; set; }

        OuterApiConfiguration OuterApiConfiguration { get; set; }

        string EsfaAdminServicesBaseUrl { get; set; }
    }
}
