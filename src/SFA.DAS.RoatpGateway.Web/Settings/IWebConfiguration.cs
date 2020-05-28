namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        AuthSettings StaffAuthentication { get; set; }

        ClientApiAuthentication ApplyApiAuthentication { get; set; }

        ClientApiAuthentication RoatpRegisterApiAuthentication { get; set; }

        string EsfaAdminServicesBaseUrl { get; set; }
    }
}
