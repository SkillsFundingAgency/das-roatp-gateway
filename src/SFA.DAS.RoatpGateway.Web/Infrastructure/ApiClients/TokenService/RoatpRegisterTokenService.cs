using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.RoatpGateway.Web.Settings;
using System;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpRegisterTokenService : IRoatpTokenService
    {
        private readonly IWebConfiguration _configuration;

        public RoatpRegisterTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            var tenantId = _configuration.RoatpRegisterApiAuthentication.TenantId;
            var clientId = _configuration.RoatpRegisterApiAuthentication.ClientId;
            var appKey = _configuration.RoatpRegisterApiAuthentication.ClientSecret;
            var resourceId = _configuration.RoatpRegisterApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
