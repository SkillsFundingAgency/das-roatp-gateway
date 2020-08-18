using SFA.DAS.RoatpGateway.Domain;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpGatewayCriminalComplianceChecksApiClient
    {
        Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId);
    }
}
