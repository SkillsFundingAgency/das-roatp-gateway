using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;

public class RoatpGatewayCriminalComplianceChecksApiClient : ApiClientBase<RoatpGatewayCriminalComplianceChecksApiClient>, IRoatpGatewayCriminalComplianceChecksApiClient
{
    public RoatpGatewayCriminalComplianceChecksApiClient(HttpClient client, ILogger<RoatpGatewayCriminalComplianceChecksApiClient> logger) : base(client, logger)
    { }

    public async Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId)
    {
        return await Get<CriminalComplianceCheckDetails>($"/Gateway/{applicationId}/CriminalCompliance/{gatewayPageId}");
    }
}
