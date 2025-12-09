using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewayApplicationActionsOrchestrator
    {
        Task<RoatpWithdrawApplicationViewModel> GetWithdrawApplicationViewModel(Guid applicationId, string userName);
        void ProcessWithdrawApplicationViewModelOnError(RoatpWithdrawApplicationViewModel viewModelOnError, ValidationResponse validationResponse);
        Task<RoatpRemoveApplicationViewModel> GetRemoveApplicationViewModel(Guid applicationId, string userId, string userName);
        void ProcessRemoveApplicationViewModelOnError(RoatpRemoveApplicationViewModel viewModelOnError, ValidationResponse validationResponse);
    }
}