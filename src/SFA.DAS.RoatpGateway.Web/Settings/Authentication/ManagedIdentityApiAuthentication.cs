using Newtonsoft.Json;

namespace SFA.DAS.RoatpGateway.Web.Settings.Authentication
{
    public class ManagedIdentityApiAuthentication : IManagedIdentityApiAuthentication
    {
        [JsonRequired] public string Identifier { get; set; }

        [JsonRequired] public string ApiBaseAddress { get; set; }
    }
}
