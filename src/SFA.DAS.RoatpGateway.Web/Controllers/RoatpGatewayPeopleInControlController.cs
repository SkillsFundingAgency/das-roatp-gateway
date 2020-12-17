using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{

    public class RoatpGatewayPeopleInControlController : RoatpGatewayControllerBase<RoatpGatewayPeopleInControlController>
    {

        private readonly IPeopleInControlOrchestrator _orchestrator;

        public RoatpGatewayPeopleInControlController(IRoatpApplicationApiClient roatpApiClient, ILogger<RoatpGatewayPeopleInControlController> logger, IRoatpGatewayPageValidator validator, IPeopleInControlOrchestrator orchestrator) : base(roatpApiClient, logger, validator)
        {
            {
                _orchestrator = orchestrator;
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/PeopleInControl")]
        public async Task<IActionResult> GetGatewayPeopleInControlPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(applicationId, username));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/PeopleInControl.cshtml"
                : $"{GatewayViewsLocation}/PeopleInControl.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/PeopleInControl")]
        public async Task<IActionResult> EvaluatePeopleInControlPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<PeopleInControlPageViewModel>> viewModelBuilder = () => _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/PeopleInControl.cshtml");
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/PeopleInControl/Clarification")]
        public async Task<IActionResult> ClarifyPeopleInControlPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<PeopleInControlPageViewModel>> viewModelBuilder = () => _orchestrator.GetPeopleInControlViewModel(new GetPeopleInControlRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/PeopleInControl.cshtml");
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/PeopleInControlRisk")]
        public async Task<IActionResult> GetGatewayPeopleInControlRiskPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetPeopleInControlHighRiskViewModel(new GetPeopleInControlHighRiskRequest(applicationId, username));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/PeopleInControlHighRisk.cshtml"
                : $"{GatewayViewsLocation}/PeopleInControlHighRisk.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/PeopleInControlRisk")]
        public async Task<IActionResult> EvaluatePeopleInControlHighRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<PeopleInControlHighRiskPageViewModel>> viewModelBuilder = () => _orchestrator.GetPeopleInControlHighRiskViewModel(new GetPeopleInControlHighRiskRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/PeopleInControlHighRisk.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/PeopleInControlRisk/Clarification")]
        public async Task<IActionResult> ClarifyPeopleInControlHighRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<PeopleInControlHighRiskPageViewModel>> viewModelBuilder = () => _orchestrator.GetPeopleInControlHighRiskViewModel(new GetPeopleInControlHighRiskRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/PeopleInControlHighRisk.cshtml");
        }
    
    }

}
