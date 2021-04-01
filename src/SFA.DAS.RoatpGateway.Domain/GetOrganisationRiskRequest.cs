using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetOrganisationRiskRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string UserName { get; }

        public GetOrganisationRiskRequest(Guid applicationId, string userId, string userName)
        {
            ApplicationId = applicationId;
            UserId = userId;
            UserName = userName;
        }
    }
}
