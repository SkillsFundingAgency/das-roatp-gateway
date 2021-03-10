using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetOneInTwelveMonthsRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetOneInTwelveMonthsRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
