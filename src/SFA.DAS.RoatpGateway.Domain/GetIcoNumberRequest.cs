using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetIcoNumberRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetIcoNumberRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
