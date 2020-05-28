using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetWebsiteRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetWebsiteRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
