using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.RoatpGateway.Web.Settings;
using System;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;

        public RoatpApplicationTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            var tenantId = _configuration.ApplyApiAuthentication.TenantId;
            var clientId = _configuration.ApplyApiAuthentication.ClientId;
            var appKey = _configuration.ApplyApiAuthentication.ClientSecret;
            var resourceId = _configuration.ApplyApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
