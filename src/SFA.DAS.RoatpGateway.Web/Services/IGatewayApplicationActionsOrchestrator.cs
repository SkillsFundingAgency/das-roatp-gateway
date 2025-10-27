using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

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