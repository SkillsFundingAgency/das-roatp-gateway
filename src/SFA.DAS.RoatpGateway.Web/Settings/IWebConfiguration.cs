using SFA.DAS.RoatpGateway.Web.Settings.Authentication;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        string SessionCachingDatabase { get; set; }

        string DataProtectionKeysDatabase { get; set; }

        AuthSettings StaffAuthentication { get; set; }

        ManagedIdentityApiAuthentication ApplyApiAuthentication { get; set; }

        ManagedIdentityApiAuthentication RoatpRegisterApiAuthentication { get; set; }

        string EsfaAdminServicesBaseUrl { get; set; }
        bool UseDfeSignIn { get; set; }
        string DfESignInServiceHelpUrl { get; set; }
    }
}
