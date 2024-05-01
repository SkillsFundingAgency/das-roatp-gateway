using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Hosting;
using SFA.DAS.RoatpGateway.Web.Settings;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService : IRoatpApplicationTokenService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IWebConfiguration _configuration;

        public RoatpApplicationTokenService(IWebHostEnvironment hostingEnvironment, IWebConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public async Task<string> GetToken(Uri baseUri)
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var generateTokenTask = azureServiceTokenProvider.GetAccessTokenAsync(_configuration.ApplyApiAuthentication.Identifier);

            return generateTokenTask.GetAwaiter().GetResult();
        }
    }
}
