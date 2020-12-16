using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.RoatpGateway.Web.Domain;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    [ExternalApiExceptionFilter]
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpGatewayApplicationActionsController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IGatewayApplicationActionsOrchestrator _applicationActionsOrchestrator;
        private readonly IRoatpRemoveApplicationViewModelValidator _removeApplicationValidator;
        private readonly IRoatpWithdrawApplicationViewModelValidator _withdrawApplicationValidator;

        public RoatpGatewayApplicationActionsController(IRoatpApplicationApiClient applyApiClient,
                                     IGatewayApplicationActionsOrchestrator applicationActionsOrchestrator,
                                     IRoatpRemoveApplicationViewModelValidator removeApplicationValidator,
                                     IRoatpWithdrawApplicationViewModelValidator withdrawApplicationValidator)
        {
            _applyApiClient = applyApiClient;
            _applicationActionsOrchestrator = applicationActionsOrchestrator;
            _removeApplicationValidator = removeApplicationValidator;
            _withdrawApplicationValidator = withdrawApplicationValidator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Remove")]
        public async Task<IActionResult> RemoveApplication(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _applicationActionsOrchestrator.GetRemoveApplicationViewModel(applicationId, username);

            if (viewModel is null)
            {
                return RedirectToAction(nameof(RoatpGatewayController.NewApplications), nameof(RoatpGatewayController));
            }

            return View("~/Views/Gateway/ConfirmRemoveApplication.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Remove")]
        public async Task<IActionResult> ConfirmRemoveApplication(Guid applicationId, RoatpRemoveApplicationViewModel viewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(RoatpGatewayController.NewApplications), nameof(RoatpGatewayController));
            }
            else if(application.OversightStatus != OversightReviewStatus.New)
            {
                return RedirectToAction(nameof(RoatpGatewayController.ViewApplication), nameof(RoatpGatewayController), new { applicationId });
            }

            var validationResponse = await _removeApplicationValidator.Validate(viewModel);
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                _applicationActionsOrchestrator.ProcessRemoveApplicationViewModelOnError(viewModel, validationResponse);
                return View("~/Views/Gateway/ConfirmRemoveApplication.cshtml", viewModel);
            }
            else if (viewModel.ConfirmApplicationAction == HtmlAndCssElements.RadioButtonValueYes)
            {
                // This would normally do something - to be implemented
                return View("~/Views/Gateway/ConfirmRemoveApplication.cshtml", viewModel);
            }
            else
            {
                return RedirectToAction(nameof(RoatpGatewayController.ViewApplication), nameof(RoatpGatewayController), new { applicationId });
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Withdraw")]
        public async Task<IActionResult> WithdrawApplication(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _applicationActionsOrchestrator.GetWithdrawApplicationViewModel(applicationId, username);

            if (viewModel is null)
            {
                return RedirectToAction(nameof(RoatpGatewayController.NewApplications), nameof(RoatpGatewayController));
            }

            return View("~/Views/Gateway/ConfirmWithdrawApplication.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Withdraw")]
        public async Task<IActionResult> ConfirmWithdrawApplication(Guid applicationId, RoatpWithdrawApplicationViewModel viewModel)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(RoatpGatewayController.NewApplications), nameof(RoatpGatewayController));
            }
            else if (application.OversightStatus != OversightReviewStatus.New)
            {
                return RedirectToAction(nameof(RoatpGatewayController.ViewApplication), nameof(RoatpGatewayController), new { applicationId });
            }

            var validationResponse = await _withdrawApplicationValidator.Validate(viewModel);
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                _applicationActionsOrchestrator.ProcessWithdrawApplicationViewModelOnError(viewModel, validationResponse);
                return View("~/Views/Gateway/ConfirmWithdrawApplication.cshtml", viewModel);
            }
            else if (viewModel.ConfirmApplicationAction == HtmlAndCssElements.RadioButtonValueYes)
            {
                // This would normally do something - to be implemented
                return View("~/Views/Gateway/ConfirmWithdrawApplication.cshtml", viewModel);
            }
            else
            {
                return RedirectToAction(nameof(RoatpGatewayController.ViewApplication), nameof(RoatpGatewayController), new { applicationId });
            }
        }
    }
}