using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class RoatpCriminalComplianceChecksController : RoatpGatewayControllerBase<RoatpCriminalComplianceChecksController>
    {
        private readonly IGatewayCriminalComplianceChecksOrchestrator _orchestrator;

        private const string CriminalComplianceView = "~/Views/Gateway/pages/CriminalComplianceChecks.cshtml";
        private const string ClarificationCriminalComplianceView = "~/Views/Gateway/pages/Clarifications/CriminalComplianceChecks.cshtml";

        public RoatpCriminalComplianceChecksController(IRoatpApplicationApiClient applyApiClient,
                                                              IRoatpGatewayPageValidator gatewayValidator,
                                                              IGatewayCriminalComplianceChecksOrchestrator orchestrator,
                                                              ILogger<RoatpCriminalComplianceChecksController> logger) : base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{gatewayPageId}")]
        public async Task<IActionResult> GetCriminalCompliancePage(Guid applicationId, string gatewayPageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(applicationId, gatewayPageId, username));
            if (viewModel.Status == SectionReviewStatus.Clarification)
                return View(ClarificationCriminalComplianceView, viewModel);
            return View(CriminalComplianceView, viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/{gatewayPageId}")]
        public async Task<IActionResult> EvaluateCriminalCompliancePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<CriminalCompliancePageViewModel>> viewModelBuilder = () => _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(command.ApplicationId, command.PageId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, CriminalComplianceView);
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/{gatewayPageId}/Clarification")]
        public async Task<IActionResult> ClarifyCriminalCompliancePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<CriminalCompliancePageViewModel>> viewModelBuilder = () => _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(command.ApplicationId, command.PageId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, ClarificationCriminalComplianceView);
        }
    }
}

