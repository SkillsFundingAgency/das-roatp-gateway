using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetRoatpRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetRoatpRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
