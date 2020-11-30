namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetGatewayApplicationCountsResponse
    {
        public int NewApplicationsCount { get; set; }
        public int InProgressApplicationsCount { get; set; }
        public int ClosedApplicationsCount { get; set; }
    }
}
