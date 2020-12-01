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
    public class RoatpOrganisationCriminalComplianceChecksController : RoatpGatewayControllerBase<RoatpOrganisationCriminalComplianceChecksController>
    {
        private readonly IGatewayCriminalComplianceChecksOrchestrator _orchestrator;

        private const string CriminalComplianceView = "~/Views/Gateway/pages/OrganisationCriminalComplianceChecks.cshtml";

        public RoatpOrganisationCriminalComplianceChecksController(IRoatpApplicationApiClient applyApiClient,
                                                              IRoatpGatewayPageValidator gatewayValidator,
                                                              IGatewayCriminalComplianceChecksOrchestrator orchestrator,
                                                              ILogger<RoatpOrganisationCriminalComplianceChecksController> logger) : base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/{gatewayPageId}")]
        public async Task<IActionResult> GetCriminalCompliancePage(Guid applicationId, string gatewayPageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(applicationId, gatewayPageId, username));

            return View(CriminalComplianceView, viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubmitComplianceCheck")]
        public async Task<IActionResult> EvaluateCriminalCompliancePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationCriminalCompliancePageViewModel>> viewModelBuilder = () => _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(command.ApplicationId, command.PageId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, CriminalComplianceView);
        }

    }
}
