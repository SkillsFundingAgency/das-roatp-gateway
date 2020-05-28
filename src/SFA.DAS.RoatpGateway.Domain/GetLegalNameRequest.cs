using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetLegalNameRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetLegalNameRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
