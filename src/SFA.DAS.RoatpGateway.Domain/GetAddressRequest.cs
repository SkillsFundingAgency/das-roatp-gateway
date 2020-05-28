using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetAddressRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetAddressRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
