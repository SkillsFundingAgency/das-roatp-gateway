using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class RoatpGatewayOrganisationChecksController : RoatpGatewayControllerBase<RoatpGatewayOrganisationChecksController>
    {
        private readonly IGatewayOrganisationChecksOrchestrator _orchestrator;

        public RoatpGatewayOrganisationChecksController(IRoatpApplicationApiClient applyApiClient,
                                                        IRoatpGatewayPageValidator gatewayValidator,
                                                        IGatewayOrganisationChecksOrchestrator orchestrator,
                                                        ILogger<RoatpGatewayOrganisationChecksController> logger) : base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, userId, username));
            if (viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy))
                return View($"{GatewayViewsLocation}/Clarifications/LegalName.cshtml", viewModel);
            return View($"{GatewayViewsLocation}/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> EvaluateLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/LegalName.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName/Clarifications")]
        public async Task<IActionResult> ClarifyLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/LegalName.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/TradingName.cshtml" 
                : $"{GatewayViewsLocation}/TradingName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> EvaluateTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/TradingName.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName/Clarifications")]
        public async Task<IActionResult> ClarifyTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/TradingName.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> GetOrganisationStatusPage(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/OrganisationStatus.cshtml"
                : $"{GatewayViewsLocation}/OrganisationStatus.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> EvaluateOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationStatus.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus/Clarifications")]
        public async Task<IActionResult> ClarifyOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OrganisationStatus.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> GetGatewayAddressPage(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetAddressViewModel(new GetAddressRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/AddressCheck.cshtml"
                : $"{GatewayViewsLocation}/AddressCheck.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> EvaluateAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/AddressCheck.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address/Clarification")]
        public async Task<IActionResult> ClarifyAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/AddressCheck.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> GetIcoNumberPage(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/IcoNumber.cshtml"
                : $"{GatewayViewsLocation}/IcoNumber.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> ClarifyIcoNumberPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<IcoNumberViewModel>> viewModelBuilder = () => _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/IcoNumber.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber/Clarificattion")]
        public async Task<IActionResult> EvaluateIcoNumberPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<IcoNumberViewModel>> viewModelBuilder = () => _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/IcoNumber.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> GetWebsitePage(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/Website.cshtml"
                : $"{GatewayViewsLocation}/Website.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> EvaluateWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Website.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress/Clarification")]
        public async Task<IActionResult> ClarifyWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/Website.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> GetOrganisationRiskPage(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(applicationId, userId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/OrganisationRisk.cshtml"
                : $"{GatewayViewsLocation}/OrganisationRisk.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> EvaluateOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationRisk.cshtml");
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk/Clarification")]
        public async Task<IActionResult> ClarifyOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            var username = HttpContext.User.UserDisplayName();
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, userId, username));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OrganisationRisk.cshtml");
        }
    }
}