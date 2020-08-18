using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetPeopleInControlRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public GetPeopleInControlRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}

