using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetOrganisationRiskRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOrganisationRiskRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
