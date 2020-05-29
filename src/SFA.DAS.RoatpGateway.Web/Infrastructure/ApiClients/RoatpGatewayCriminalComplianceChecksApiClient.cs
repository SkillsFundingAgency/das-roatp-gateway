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
        private readonly ITokenService _tokenService;

        public RoatpGatewayCriminalComplianceChecksApiClient(HttpClient client, ILogger<RoatpGatewayCriminalComplianceChecksApiClient> logger, ITokenService tokenService)
            : base(client, logger)
        {
            _client = client;
            _tokenService = tokenService;
        }

        public async Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId)
        {
            return await Get<CriminalComplianceCheckDetails>($"/Gateway/{applicationId}/CriminalCompliance/{gatewayPageId}");
        }
    }
}
