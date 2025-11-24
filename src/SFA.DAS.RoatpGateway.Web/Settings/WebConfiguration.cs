using Newtonsoft.Json;

namespace SFA.DAS.RoatpGateway.Web.Settings;

public class WebConfiguration : IWebConfiguration
{
    [JsonRequired]
    public string SessionRedisConnectionString { get; set; }

    [JsonRequired]
    public string SessionCachingDatabase { get; set; }

    [JsonRequired]
    public string DataProtectionKeysDatabase { get; set; }

    [JsonRequired]
    public InnerApiConfiguration ApplyApiAuthentication { get; set; }

    [JsonRequired]
    public InnerApiConfiguration RoatpRegisterApiAuthentication { get; set; }

    [JsonRequired]
    public string EsfaAdminServicesBaseUrl { get; set; }
    public string DfESignInServiceHelpUrl { get; set; }
}
