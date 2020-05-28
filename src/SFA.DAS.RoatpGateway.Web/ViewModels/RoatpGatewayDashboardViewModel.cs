using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayDashboardViewModel
    {
        public PaginatedList<RoatpApplicationSummaryItem> Applications { get; set; }
    }
}
