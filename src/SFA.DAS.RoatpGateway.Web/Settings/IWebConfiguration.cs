namespace SFA.DAS.RoatpGateway.Web.Settings;

public interface IWebConfiguration
{
    string SessionRedisConnectionString { get; set; }

    string SessionCachingDatabase { get; set; }

    string DataProtectionKeysDatabase { get; set; }

    InnerApiConfiguration ApplyApiAuthentication { get; set; }

    InnerApiConfiguration RoatpRegisterApiAuthentication { get; set; }

    string EsfaAdminServicesBaseUrl { get; set; }
    string DfESignInServiceHelpUrl { get; set; }
}
