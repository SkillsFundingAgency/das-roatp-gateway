using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpGatewayControllerTests : RoatpGatewayControllerTestBase<RoatpGatewayController>
    {
        private RoatpGatewayController _controller;
        private Mock<IGatewayOverviewOrchestrator> _orchestrator;
        private Mock<IRoatpGatewayApplicationViewModelValidator> _validator;
        private Mock<IRoatpGatewayPageValidator> _pageValidator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOverviewOrchestrator>();
            _validator = new Mock<IRoatpGatewayApplicationViewModelValidator>();
            _pageValidator = new Mock<IRoatpGatewayPageValidator>();
            _controller = new RoatpGatewayController(ApplyApiClient.Object, _orchestrator.Object,
                                                     _validator.Object, Logger.Object, _pageValidator.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task ConfirmOutcome_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpGatewayApplicationViewModel { ReadyToConfirm = true };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new RoatpApplicationResponse { ApplicationId = applicationId });
            _orchestrator.Setup(x => x.GetConfirmOverviewViewModel(It.Is<GetApplicationOverviewRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.ConfirmOutcome(applicationId, GatewayReviewStatus.Pass, null);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModel, viewResult.Model);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_pass_result()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Pass,
                OptionApprovedText = "Some approved text",
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateConfirmOutcome(viewModel);

            _orchestrator.Verify(x => x.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, Username)), Times.Never);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_fail_result()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Fail,
                OptionApprovedText = "Some approved text",
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateConfirmOutcome(viewModel);

            _orchestrator.Verify(x => x.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, Username)), Times.Never);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_reject_result()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Reject,
                OptionApprovedText = "Some approved text",
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateConfirmOutcome(viewModel);

            _orchestrator.Verify(x => x.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, Username)), Times.Never);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_result_is_on_error()
        {
            var applicationId = Guid.NewGuid();
            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new RoatpApplicationResponse { ApplicationId = applicationId });

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "GatewayReviewStatus", ErrorMessage = "Select what you want to do"}
                        }
                });

            var expectedViewModelWithErrors = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.New,
                ErrorMessages = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "GatewayReviewStatus", ErrorMessage = "Select what you want to do"}
                        }
            };

            _orchestrator.Setup(x => x.GetConfirmOverviewViewModel(It.Is<GetApplicationOverviewRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModelWithErrors);

            var result = await _controller.EvaluateConfirmOutcome(viewModel);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModelWithErrors, viewResult.Model);
        }

        [Test]
        public async Task AboutToConfirmOutcome_selection_Yes()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayConfirmOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Pass,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "Yes"
            };

            var result = await _controller.AboutToConfirmOutcome(viewModel);
            var viewResult = result as ViewResult;
            var viewResultModel = viewResult.Model as RoatpGatewayOutcomeViewModel;

            Assert.AreSame(viewModel.GatewayReviewStatus, viewResultModel.GatewayReviewStatus);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Once);
        }

        [Test]
        public async Task AboutToConfirmOutcome_selection_No()
        {
            var applicationId = Guid.NewGuid();
            var expectedActionName = "ConfirmOutcome";
            var viewModel = new RoatpGatewayConfirmOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Pass,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "No"
            };

            var result = await _controller.AboutToConfirmOutcome(viewModel);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.AreSame(expectedActionName, redirectToActionResult.ActionName);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToConfirmOutcome_no_selection()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayConfirmOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Pass,
                GatewayReviewComment = "some comment"
            };

            _controller.ModelState.AddModelError("ConfirmGatewayOutcome", "Select if you are sure you want to pass this application");

            var result = await _controller.AboutToConfirmOutcome(viewModel);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayConfirmOutcomeViewModel;

            Assert.AreSame(HtmlAndCssElements.CssFormGroupErrorClass, resultViewModel.CssFormGroupError);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }



        [Test]
        public async Task AboutToFailOutcome_selection_Yes()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayFailOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Fail,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "Yes"
            };

            var result = await _controller.AboutToFailOutcome(viewModel);
            var viewResult = result as ViewResult;
            var viewResultModel = viewResult.Model as RoatpGatewayOutcomeViewModel;

            Assert.AreSame(viewModel.GatewayReviewStatus, viewResultModel.GatewayReviewStatus);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Once);
        }

        [Test]
        public async Task AboutToFailOutcome_selection_No()
        {
            var applicationId = Guid.NewGuid();
            var expectedActionName = "ConfirmOutcome";
            var viewModel = new RoatpGatewayFailOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Pass,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "No"
            };

            var result = await _controller.AboutToFailOutcome(viewModel);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.AreSame(expectedActionName, redirectToActionResult.ActionName);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToFailOutcome_no_selection()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayFailOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Reject,
                GatewayReviewComment = "some comment"
            };

            _controller.ModelState.AddModelError("ConfirmGatewayOutcome", "Select if you are sure you want to pass this application");

            var result = await _controller.AboutToFailOutcome(viewModel);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayFailOutcomeViewModel;

            Assert.AreSame(HtmlAndCssElements.CssFormGroupErrorClass, resultViewModel.CssFormGroupError);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToRejectOutcome_selection_Yes()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayRejectOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Reject,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "Yes"
            };

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var viewResult = result as ViewResult;
            var viewResultModel = viewResult.Model as RoatpGatewayOutcomeViewModel;

            Assert.AreSame(viewModel.GatewayReviewStatus, viewResultModel.GatewayReviewStatus);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Once);
        }

        [Test]
        public async Task AboutToRejectOutcome_selection_No()
        {
            var applicationId = Guid.NewGuid();
            var expectedActionName = "ConfirmOutcome";
            var viewModel = new RoatpGatewayRejectOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Reject,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "No"
            };

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.AreSame(expectedActionName, redirectToActionResult.ActionName);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToRejectOutcome_no_selection()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayRejectOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Reject,
                GatewayReviewComment = "some comment"
            };

            _controller.ModelState.AddModelError("ConfirmGatewayOutcome", "Select if you are sure you want to reject this application");

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayRejectOutcomeViewModel;

            Assert.AreSame(HtmlAndCssElements.CssFormGroupErrorClass, resultViewModel.CssFormGroupError);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, UserId, Username), Times.Never);
        }

        [Test]
        public async Task NewApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetNewGatewayApplications()).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts()).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.NewApplications(1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.AreEqual(1, resultViewModel.ApplicationCounts.NewApplicationsCount);
            Assert.AreEqual(2, resultViewModel.ApplicationCounts.InProgressApplicationsCount);
            Assert.AreEqual(3, resultViewModel.ApplicationCounts.ClosedApplicationsCount);
        }

        [Test]
        public async Task InProgressApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetInProgressGatewayApplications()).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts()).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.InProgressApplications(1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.AreEqual(1, resultViewModel.ApplicationCounts.NewApplicationsCount);
            Assert.AreEqual(2, resultViewModel.ApplicationCounts.InProgressApplicationsCount);
            Assert.AreEqual(3, resultViewModel.ApplicationCounts.ClosedApplicationsCount);
        }

        [Test]
        public async Task ClosedApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetClosedGatewayApplications()).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts()).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.ClosedApplications(1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.AreEqual(1, resultViewModel.ApplicationCounts.NewApplicationsCount);
            Assert.AreEqual(2, resultViewModel.ApplicationCounts.InProgressApplicationsCount);
            Assert.AreEqual(3, resultViewModel.ApplicationCounts.ClosedApplicationsCount);
        }
    }
}
