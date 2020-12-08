using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/TwoInTwelveMonths")]
        public async Task<IActionResult> GetTwoInTwelveMonthsPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTwoInTwelveMonthsViewModel(new GetTwoInTwelveMonthsRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/TwoInTwelveMonths.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TwoInTwelveMonths")]
        public async Task<IActionResult> EvaluateTwoInTwelveMonthsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<TwoInTwelveMonthsViewModel>> viewModelBuilder = () => _orchestrator.GetTwoInTwelveMonthsViewModel(new GetTwoInTwelveMonthsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/TwoInTwelveMonths.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> GetGatewayLegalNamePage(Guid applicationId, string pageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/LegalName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/LegalName")]
        public async Task<IActionResult> EvaluateLegalNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<LegalNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetLegalNameViewModel(new GetLegalNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/LegalName.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> GetGatewayTradingNamePage(Guid applicationId, string pageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/TradingName.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/TradingName")]
        public async Task<IActionResult> EvaluateTradingNamePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<TradingNamePageViewModel>> viewModelBuilder = () => _orchestrator.GetTradingNameViewModel(new GetTradingNameRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/TradingName.cshtml");
        }


        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> GetOrganisationStatusPage(Guid applicationId, string pageId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/OrganisationStatus.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationStatus")]
        public async Task<IActionResult> EvaluateOrganisationStatus(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationStatusViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationStatus.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> GetGatewayAddressPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetAddressViewModel(new GetAddressRequest(applicationId, username));
            return View("~/Views/Gateway/pages/AddressCheck.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Address")]
        public async Task<IActionResult> EvaluateAddressPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<AddressCheckViewModel>> viewModelBuilder = () => _orchestrator.GetAddressViewModel(new GetAddressRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/AddressCheck.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
        public async Task<IActionResult> GetIcoNumberPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetIcoNumberViewModel(new GetIcoNumberRequest(applicationId, username));
            return View("~/Views/Gateway/pages/IcoNumber.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/IcoNumber")]
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
            return View($"{GatewayViewsLocation}/Website.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/WebsiteAddress")]
        public async Task<IActionResult> EvaluateWebsitePage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<WebsiteViewModel>> viewModelBuilder = () => _orchestrator.GetWebsiteViewModel(new GetWebsiteRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Website.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> GetOrganisationRiskPage(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(applicationId, username));
            return View($"{GatewayViewsLocation}/OrganisationRisk.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OrganisationRisk")]
        public async Task<IActionResult> EvaluateOrganisationRiskPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OrganisationRiskViewModel>> viewModelBuilder = () => _orchestrator.GetOrganisationRiskViewModel(new GetOrganisationRiskRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OrganisationRisk.cshtml");
        }
    }
}