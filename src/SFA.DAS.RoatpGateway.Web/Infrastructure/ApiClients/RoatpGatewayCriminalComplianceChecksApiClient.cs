using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpGatewayCriminalComplianceChecksApiClient : ApiClientBase<RoatpGatewayCriminalComplianceChecksApiClient>, IRoatpGatewayCriminalComplianceChecksApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpGatewayCriminalComplianceChecksApiClient> _logger;
        private readonly ITokenService _tokenService;

        public RoatpGatewayCriminalComplianceChecksApiClient(HttpClient client, ILogger<RoatpGatewayCriminalComplianceChecksApiClient> logger, ITokenService tokenService)
            : base(client, logger)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId)
        {
            return await Get<CriminalComplianceCheckDetails>($"/Gateway/{applicationId}/CriminalCompliance/{gatewayPageId}");
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken(_client.BaseAddress));

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }
    }
}
