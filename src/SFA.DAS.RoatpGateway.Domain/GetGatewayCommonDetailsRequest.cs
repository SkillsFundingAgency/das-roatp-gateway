namespace SFA.DAS.RoatpGateway.Domain
{
    public class GatewayCommonDetailsRequest
    {
        public string PageId { get; }
        public string UserId { get; }
        public string UserName { get; }

        public GatewayCommonDetailsRequest(string pageId, string userId, string userName)
        {
            PageId = pageId;
            UserId = userId;
            UserName = userName;
        }
    }
}
