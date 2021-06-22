using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayDashboardViewModel
    {
        public PaginatedList<RoatpApplicationSummaryItem> Applications { get; set; }
        public GetGatewayApplicationCountsResponse ApplicationCounts { get; set; }
        public string SelectedTab { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
