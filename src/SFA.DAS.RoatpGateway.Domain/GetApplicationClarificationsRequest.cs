using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetApplicationClarificationsRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetApplicationClarificationsRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}