using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services;

public interface IGatewayOverviewOrchestrator
{
    Task<RoatpGatewayApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request);
    Task<RoatpGatewayClarificationsViewModel> GetClarificationViewModel(GetApplicationClarificationsRequest request);
    Task<RoatpGatewayApplicationViewModel> GetConfirmOverviewViewModel(GetApplicationOverviewRequest request);
    void ProcessViewModelOnError(RoatpGatewayApplicationViewModel viewModelOnError, RoatpGatewayApplicationViewModel viewModel, ValidationResponse validationResponse);
}
