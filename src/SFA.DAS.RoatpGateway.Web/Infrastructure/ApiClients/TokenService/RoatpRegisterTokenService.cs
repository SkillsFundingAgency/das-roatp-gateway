using System;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.RoatpGateway.Web.Settings;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpRegisterTokenService : IRoatpRegisterTokenService
    {
        private readonly IWebConfiguration _configuration;

        public RoatpRegisterTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            return await new AzureServiceTokenProvider().GetAccessTokenAsync(_configuration.RoatpRegisterApiAuthentication.Identifier);
        }
    }
}
