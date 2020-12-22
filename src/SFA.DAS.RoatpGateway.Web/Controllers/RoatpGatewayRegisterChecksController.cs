using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class RoatpGatewayRegisterChecksController : RoatpGatewayControllerBase<RoatpGatewayRegisterChecksController>
    {
        private readonly IGatewayRegisterChecksOrchestrator _orchestrator;

        public RoatpGatewayRegisterChecksController(IRoatpApplicationApiClient applyApiClient,
                                                IRoatpGatewayPageValidator gatewayValidator,
                                                IGatewayRegisterChecksOrchestrator orchestrator,
                                                ILogger<RoatpGatewayRegisterChecksController> logger) : base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> GetGatewayRoatpPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoatpViewModel(new GetRoatpRequest(applicationId, username));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/Roatp.cshtml"
                : $"{GatewayViewsLocation}/Roatp.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> EvaluateRoatpPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoatpPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoatpViewModel(new GetRoatpRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Roatp.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roatp/Clarification")]
        public async Task<IActionResult> ClarifyRoatpPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoatpPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoatpViewModel(new GetRoatpRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/Roatp.cshtml");
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> GetGatewayRoepaoPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(applicationId, username));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/Roepao.cshtml"
                : $"{GatewayViewsLocation}/Roepao.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> EvaluateRoepaoPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoepaoPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Roepao.cshtml");
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roepao/Clarification")]
        public async Task<IActionResult> ClarifyRoepaoPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoepaoPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/Roepao.cshtml");
        }
    }
}
