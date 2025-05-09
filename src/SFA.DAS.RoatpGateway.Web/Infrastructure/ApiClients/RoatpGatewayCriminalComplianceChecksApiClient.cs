using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpGatewayCriminalComplianceChecksApiClient : ApiClientBase<RoatpGatewayCriminalComplianceChecksApiClient>, IRoatpGatewayCriminalComplianceChecksApiClient
    {
        public RoatpGatewayCriminalComplianceChecksApiClient(HttpClient client, ILogger<RoatpGatewayCriminalComplianceChecksApiClient> logger, IRoatpApplicationTokenService tokenService)
            : base(client, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(client.BaseAddress).Result);
        }

        public async Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId)
        {
            return await Get<CriminalComplianceCheckDetails>($"/Gateway/{applicationId}/CriminalCompliance/{gatewayPageId}");
        }
    }
}
