using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.RoatpGateway.Web.Settings;

public interface IWebConfiguration
{
    string SessionRedisConnectionString { get; set; }

    string SessionCachingDatabase { get; set; }

    string DataProtectionKeysDatabase { get; set; }

    ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

    ClientApiAuthentication RoatpRegisterApiAuthentication { get; set; }

    string EsfaAdminServicesBaseUrl { get; set; }
    string DfESignInServiceHelpUrl { get; set; }
}
