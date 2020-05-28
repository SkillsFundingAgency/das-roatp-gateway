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

        private const string GatewayViewsLocation = "~/Views/Gateway/pages";

        public RoatpGatewayRegisterChecksController(IRoatpApplicationApiClient applyApiClient,
                                                IHttpContextAccessor contextAccessor,
                                                IRoatpGatewayPageValidator gatewayValidator,
                                                IGatewayRegisterChecksOrchestrator orchestrator,
                                                ILogger<RoatpGatewayRegisterChecksController> logger) : base(contextAccessor, applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> GetGatewayRoatpPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoatpViewModel(new GetRoatpRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/Roatp.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roatp")]
        public async Task<IActionResult> EvaluateRoatpPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoatpPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoatpViewModel(new GetRoatpRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Roatp.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> GetGatewayRoepaoPage(Guid applicationId, string pageId)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/Roepao.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Roepao")]
        public async Task<IActionResult> EvaluateRoepaoPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<RoepaoPageViewModel>> viewModelBuilder = () => _orchestrator.GetRoepaoViewModel(new GetRoepaoRequest(command.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Roepao.cshtml");
        }
    }
}
