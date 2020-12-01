using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetTwoInTwelveMonthsRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetTwoInTwelveMonthsRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
