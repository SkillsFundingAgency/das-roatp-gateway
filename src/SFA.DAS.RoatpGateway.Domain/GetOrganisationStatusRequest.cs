using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetOrganisationStatusRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOrganisationStatusRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
