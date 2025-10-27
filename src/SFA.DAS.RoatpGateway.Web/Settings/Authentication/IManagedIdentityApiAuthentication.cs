namespace SFA.DAS.RoatpGateway.Web.Settings.Authentication
{
    public interface IManagedIdentityApiAuthentication
    {
        string Identifier { get; set; }
        string ApiBaseAddress { get; set; }
    }
}
