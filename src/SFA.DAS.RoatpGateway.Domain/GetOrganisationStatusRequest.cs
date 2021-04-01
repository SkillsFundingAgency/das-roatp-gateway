using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetOrganisationStatusRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string UserName { get; }

        public GetOrganisationStatusRequest(Guid applicationId, string userId, string userName)
        {
            ApplicationId = applicationId;
            UserId = userId;
            UserName = userName;
        }
    }
}
