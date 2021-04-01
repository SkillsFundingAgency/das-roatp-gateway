using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetCriminalComplianceCheckRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string UserName { get; }

        public string PageId { get; set; }

        public GetCriminalComplianceCheckRequest(Guid applicationId, string pageId, string userId, string username)
        {
            ApplicationId = applicationId;
            PageId = pageId;
            UserId = userId;
            UserName = username;
        }
    }
}
