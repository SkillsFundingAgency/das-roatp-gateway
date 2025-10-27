namespace SFA.DAS.RoatpGateway.Web.Settings.Authentication
{
    public interface IClientApiAuthentication
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Instance { get; set; }
        string ResourceId { get; set; }
        string TenantId { get; set; }
        string ApiBaseAddress { get; set; }
    }
}
