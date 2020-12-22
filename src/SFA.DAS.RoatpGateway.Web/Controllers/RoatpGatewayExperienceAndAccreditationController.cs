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
    public class RoatpGatewayExperienceAndAccreditationController : RoatpGatewayControllerBase<RoatpGatewayExperienceAndAccreditationController>
    {
        private readonly IGatewayExperienceAndAccreditationOrchestrator _orchestrator;

        public RoatpGatewayExperienceAndAccreditationController(IRoatpApplicationApiClient roatpApiClient, IRoatpGatewayPageValidator validator, IGatewayExperienceAndAccreditationOrchestrator orchestrator, ILogger<RoatpGatewayExperienceAndAccreditationController> logger) : base(roatpApiClient, logger, validator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<ViewResult> SubcontractorDeclaration(Guid applicationId)
        {
            var userName = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(applicationId, userName));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/SubcontractorDeclaration.cshtml"
                : $"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclarationFile")]
        public async Task<FileStreamResult> SubcontractorDeclarationContractFile(Guid applicationId)
        {
            return await _orchestrator.GetSubcontractorDeclarationContractFile(new GetSubcontractorDeclarationContractFileRequest(applicationId));
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration")]
        public async Task<IActionResult> EvaluateSubcontractorDeclarationPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<SubcontractorDeclarationViewModel>> viewModelBuilder = () => _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/SubcontractorDeclaration.cshtml");
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/Page/SubcontractorDeclaration/Clarification")]
        public async Task<IActionResult> ClarifySubcontractorDeclarationPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<SubcontractorDeclarationViewModel>> viewModelBuilder = () => _orchestrator.GetSubcontractorDeclarationViewModel(new GetSubcontractorDeclarationRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/SubcontractorDeclaration.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/OfficeForStudents")]
        public async Task<ViewResult> OfficeForStudents(Guid applicationId)
        {
            var userName = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOfficeForStudentsViewModel(new GetOfficeForStudentsRequest(applicationId, userName));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/OfficeForStudents.cshtml"
                : $"{GatewayViewsLocation}/OfficeForStudents.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OfficeForStudents")]
        public async Task<IActionResult> EvaluateOfficeForStudentsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfficeForStudentsViewModel>> viewModelBuilder = () => _orchestrator.GetOfficeForStudentsViewModel(new GetOfficeForStudentsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OfficeForStudents.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/OfficeForStudents/Clarification")]
        public async Task<IActionResult> ClarifyOfficeForStudentsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfficeForStudentsViewModel>> viewModelBuilder = () => _orchestrator.GetOfficeForStudentsViewModel(new GetOfficeForStudentsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OfficeForStudents.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/InitialTeacherTraining")]
        public async Task<ViewResult> InitialTeacherTraining(Guid applicationId)
        {
            var userName = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetInitialTeacherTrainingViewModel(new GetInitialTeacherTrainingRequest(applicationId, userName));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/InitialTeacherTraining.cshtml"
                : $"{GatewayViewsLocation}/InitialTeacherTraining.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/InitialTeacherTraining")]
        public async Task<IActionResult> EvaluateInitialTeacherTrainingPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<InitialTeacherTrainingViewModel>> viewModelBuilder = () => _orchestrator.GetInitialTeacherTrainingViewModel(new GetInitialTeacherTrainingRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/InitialTeacherTraining.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/InitialTeacherTraining/Clarification")]
        public async Task<IActionResult> ClarifyInitialTeacherTrainingPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<InitialTeacherTrainingViewModel>> viewModelBuilder = () => _orchestrator.GetInitialTeacherTrainingViewModel(new GetInitialTeacherTrainingRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/InitialTeacherTraining.cshtml");
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Page/Ofsted")]
        public async Task<ViewResult> OfstedDetails(Guid applicationId)
        {
            var userName = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetOfstedDetailsViewModel(new GetOfstedDetailsRequest(applicationId, userName));
            return View(viewModel.Status == SectionReviewStatus.Clarification
                ? $"{GatewayViewsLocation}/Clarifications/OfstedDetails.cshtml"
                : $"{GatewayViewsLocation}/OfstedDetails.cshtml", viewModel);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Ofsted")]
        public async Task<IActionResult> EvaluateOfstedDetailsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfstedDetailsViewModel>> viewModelBuilder = () => _orchestrator.GetOfstedDetailsViewModel(new GetOfstedDetailsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/OfstedDetails.cshtml");
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Page/Ofsted/Clarification")]
        public async Task<IActionResult> ClarifyOfstedDetailsPage(SubmitGatewayPageAnswerCommand command)
        {
            Func<Task<OfstedDetailsViewModel>> viewModelBuilder = () => _orchestrator.GetOfstedDetailsViewModel(new GetOfstedDetailsRequest(command.ApplicationId, HttpContext.User.UserDisplayName()));
            return await ValidateAndUpdateClarificationPageAnswer(command, viewModelBuilder, $"{GatewayViewsLocation}/Clarifications/OfstedDetails.cshtml");
        }
    }
}