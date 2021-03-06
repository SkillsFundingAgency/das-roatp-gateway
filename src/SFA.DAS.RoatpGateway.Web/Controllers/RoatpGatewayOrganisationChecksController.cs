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


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OneInTwelveMonths")]
        public async Task<IActionResult> GetOneInTwelveMonthsPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOneInTwelveMonthsViewModel(new GetOneInTwelveMonthsRequest(applicationId, username));
            if (viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy))
                return View($"{GatewayViewsLocation}/Clarifications/OneInTwelveMonths.cshtml", viewModel);
            return View($"{GatewayViewsLocation}/OneInTwelveMonths.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OneInTwelveMonths")]
        public async Task<IActionResult> EvaluateOneInTwelveMonthsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OneInTwelveMonthsViewModel>> viewModelBuilder = () => _orchestrator.GetOneInTwelveMonthsViewModel(new GetOneInTwelveMonthsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OneInTwelveMonths.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OneInTwelveMonths/Clarification")]
        public async Task<IActionResult> ClarifyOneInTwelveMonthsPage(SubmitGatewayPageAnswerCommand command)
        {
            
            Func<Task<OneInTwelveMonthsViewModel>> viewModelBuilder = () => _orchestrator.GetOneInTwelveMonthsViewModel(new GetOneInTwelveMonthsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OneInTwelveMonths.cshtml");
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username));
            if (viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy))
                return View($"{GatewayViewsLocation}/Clarifications/LegalName.cshtml", viewModel);
            return View($"{GatewayViewsLocation}/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> EvaluateLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/LegalName.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName/Clarifications")]
        public async Task<IActionResult> ClarifyLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/LegalName.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/TradingName.cshtml" 
                : $"{GatewayViewsLocation}/TradingName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> EvaluateTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/TradingName.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName/Clarifications")]
        public async Task<IActionResult> ClarifyTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/TradingName.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> GetOrganisationStatusPage(Guid applicationId, string pageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/OrganisationStatus.cshtml"
                : $"{GatewayViewsLocation}/OrganisationStatus.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> EvaluateOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationStatus.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus/Clarifications")]
        public async Task<IActionResult> ClarifyOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OrganisationStatus.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> GetGatewayAddressPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetAddressViewModel(new GetAddressRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/AddressCheck.cshtml"
                : $"{GatewayViewsLocation}/AddressCheck.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> EvaluateAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/AddressCheck.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address/Clarification")]
        public async Task<IActionResult> ClarifyAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/AddressCheck.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> GetIcoNumberPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/IcoNumber.cshtml"
                : $"{GatewayViewsLocation}/IcoNumber.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> ClarifyIcoNumberPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<IcoNumberViewModel>> viewModelBuilder = () => _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/IcoNumber.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber/Clarificattion")]
        public async Task<IActionResult> EvaluateIcoNumberPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<IcoNumberViewModel>> viewModelBuilder = () => _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/IcoNumber.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> GetWebsitePage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/Website.cshtml"
                : $"{GatewayViewsLocation}/Website.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> EvaluateWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Website.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress/Clarification")]
        public async Task<IActionResult> ClarifyWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/Website.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> GetOrganisationRiskPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(applicationId, username));
            return View(viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent && !string.IsNullOrEmpty(viewModel.ClarificationBy)
                ? $"{GatewayViewsLocation}/Clarifications/OrganisationRisk.cshtml"
                : $"{GatewayViewsLocation}/OrganisationRisk.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> EvaluateOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationRisk.cshtml");
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk/Clarification")]
        public async Task<IActionResult> ClarifyOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OrganisationRisk.cshtml");
        }
    }
}