using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetApplicationOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetApplicationOverviewRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
