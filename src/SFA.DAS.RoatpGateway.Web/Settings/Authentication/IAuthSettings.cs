namespace SFA.DAS.RoatpGateway.Web.Settings.Authentication
{
    public interface IAuthSettings
    {
        string WtRealm { get; set; }
        string MetadataAddress { get; set; }
        string Role { get; set; }
    }
}
