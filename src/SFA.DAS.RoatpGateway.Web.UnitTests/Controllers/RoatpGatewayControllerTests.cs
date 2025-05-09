using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpGatewayControllerTests : RoatpGatewayControllerTestBase<RoatpGatewayController>
    {
        private RoatpGatewayController _controller;
        private Mock<IGatewayOverviewOrchestrator> _orchestrator;
        private Mock<IRoatpGatewayApplicationViewModelValidator> _validator;
        private Mock<IRoatpSearchTermValidator> _searchTermValidator;
        private Mock<IRoatpGatewayPageValidator> _pageValidator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOverviewOrchestrator>();
            _validator = new Mock<IRoatpGatewayApplicationViewModelValidator>();
            _searchTermValidator = new Mock<IRoatpSearchTermValidator>();
            _pageValidator = new Mock<IRoatpGatewayPageValidator>();

            _controller = new RoatpGatewayController(ApplyApiClient.Object, _orchestrator.Object,
                                                     _validator.Object, _searchTermValidator.Object,
                                                     Logger.Object, _pageValidator.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task ConfirmOutcome_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpGatewayApplicationViewModel { ReadyToConfirm = true };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _orchestrator.Setup(x => x.GetConfirmOverviewViewModel(It.Is<GetApplicationOverviewRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.ConfirmOutcome(applicationId, GatewayReviewStatus.Pass, null, null, 0);
            var viewResult = result as ViewResult;
            Assert.That(expectedViewModel, Is.SameAs(viewResult.Model));
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
                GatewayReviewStatus = GatewayReviewStatus.Rejected,
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
            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId, ApplyData = new ApplyData { ApplyDetails = new ApplyDetails { } } });

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
            Assert.That(expectedViewModelWithErrors, Is.SameAs(viewResult.Model));
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

            Assert.That(viewModel.GatewayReviewStatus, Is.SameAs(viewResultModel.GatewayReviewStatus));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, string.Empty, viewModel.SubcontractingLimit, UserId, Username), Times.Once);
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

            Assert.That(expectedActionName, Is.SameAs(redirectToActionResult.ActionName));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
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

            Assert.That(HtmlAndCssElements.CssFormGroupErrorClass, Is.SameAs(resultViewModel.CssFormGroupError));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
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
                GatewayReviewExternalComment = "some external comment",
                ConfirmGatewayOutcome = "Yes"
            };

            var result = await _controller.AboutToFailOutcome(viewModel);
            var viewResult = result as ViewResult;
            var viewResultModel = viewResult.Model as RoatpGatewayOutcomeViewModel;

            Assert.That(viewModel.GatewayReviewStatus, Is.SameAs(viewResultModel.GatewayReviewStatus));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Once);
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

            Assert.That(expectedActionName, Is.SameAs(redirectToActionResult.ActionName));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToFailOutcome_no_selection()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayFailOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Rejected,
                GatewayReviewComment = "some comment"
            };

            _controller.ModelState.AddModelError("ConfirmGatewayOutcome", "Select if you are sure you want to pass this application");

            var result = await _controller.AboutToFailOutcome(viewModel);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayFailOutcomeViewModel;

            Assert.That(HtmlAndCssElements.CssFormGroupErrorClass, Is.SameAs(resultViewModel.CssFormGroupError));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToRejectOutcome_selection_Yes()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayRejectedOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Rejected,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "Yes"
            };

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var viewResult = result as ViewResult;
            var viewResultModel = viewResult.Model as RoatpGatewayOutcomeViewModel;

            Assert.That(viewModel.GatewayReviewStatus, Is.SameAs(viewResultModel.GatewayReviewStatus));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Once);
        }

        [Test]
        public async Task AboutToRejectOutcome_selection_No()
        {
            var applicationId = Guid.NewGuid();
            var expectedActionName = "ConfirmOutcome";
            var viewModel = new RoatpGatewayRejectedOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Rejected,
                GatewayReviewComment = "some comment",
                ConfirmGatewayOutcome = "No"
            };

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.That(expectedActionName, Is.SameAs(redirectToActionResult.ActionName));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
        }

        [Test]
        public async Task AboutToRejectOutcome_no_selection()
        {
            var applicationId = Guid.NewGuid();
            var viewModel = new RoatpGatewayRejectedOutcomeViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Rejected,
                GatewayReviewComment = "some comment"
            };

            _controller.ModelState.AddModelError("ConfirmGatewayOutcome", "Select if you are sure you want to reject this application");

            var result = await _controller.AboutToRejectOutcome(viewModel);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayRejectedOutcomeViewModel;

            Assert.That(HtmlAndCssElements.CssFormGroupErrorClass, Is.SameAs(resultViewModel.CssFormGroupError));
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAndComment(applicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, UserId, Username), Times.Never);
        }

        [Test]
        public async Task NewApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetNewGatewayApplications(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts(It.IsAny<string>())).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.NewApplications(null, "", 1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.That(1, Is.EqualTo(resultViewModel.ApplicationCounts.NewApplicationsCount));
            Assert.That(2, Is.EqualTo(resultViewModel.ApplicationCounts.InProgressApplicationsCount));
            Assert.That(3, Is.EqualTo(resultViewModel.ApplicationCounts.ClosedApplicationsCount));
        }

        [Test]
        public async Task InProgressApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetInProgressGatewayApplications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts(It.IsAny<string>())).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.InProgressApplications(null, "", "", 1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.That(1, Is.EqualTo(resultViewModel.ApplicationCounts.NewApplicationsCount));
            Assert.That(2, Is.EqualTo(resultViewModel.ApplicationCounts.InProgressApplicationsCount));
            Assert.That(3, Is.EqualTo(resultViewModel.ApplicationCounts.ClosedApplicationsCount));
        }

        [Test]
        public async Task ClosedApplications_ViewModel_Has_Correct_Application_Counts()
        {
            ApplyApiClient.Setup(x => x.GetClosedGatewayApplications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<RoatpApplicationSummaryItem>());

            ApplyApiClient.Setup(x => x.GetApplicationCounts(It.IsAny<string>())).ReturnsAsync(new GetGatewayApplicationCountsResponse
            {
                NewApplicationsCount = 1,
                InProgressApplicationsCount = 2,
                ClosedApplicationsCount = 3
            });

            var result = await _controller.ClosedApplications(null, "", "", 1);
            var viewResult = result as ViewResult;
            var resultViewModel = viewResult.Model as RoatpGatewayDashboardViewModel;

            Assert.That(1, Is.EqualTo(resultViewModel.ApplicationCounts.NewApplicationsCount));
            Assert.That(2, Is.EqualTo(resultViewModel.ApplicationCounts.InProgressApplicationsCount));
            Assert.That(3, Is.EqualTo(resultViewModel.ApplicationCounts.ClosedApplicationsCount));
        }


        [Test]
        public async Task AskForClarification_no_view_model_returned()
        {
            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync((RoatpGatewayClarificationsViewModel)null);

            var result = await _controller.AskForClarification(new Guid());
            var viewResult = result as RedirectToActionResult;
            Assert.That("NewApplications", Is.EqualTo(viewResult.ActionName));
        }

        [Test]
        public async Task AskForClarification_view_model_returned()
        {
            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync(new RoatpGatewayClarificationsViewModel());

            var result = await _controller.AskForClarification(new Guid());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewName.Contains("AskForClarification.cshtml"), Is.True);
        }



        [Test]
        public async Task AboutToAskForClarification_with_errors_no_view_model_returned()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync((RoatpGatewayClarificationsViewModel)null);

            var confirmAskForClarification = string.Empty;
            var result = await _controller.AboutToAskForClarification(applicationId, confirmAskForClarification);
            var viewResult = result as RedirectToActionResult;
            Assert.That("NewApplications", Is.EqualTo(viewResult.ActionName));
        }

        [Test]
        public async Task AboutToAskForClarification_with_errors_view_model_returned()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync(new RoatpGatewayClarificationsViewModel());

            var confirmAskForClarification = string.Empty;
            var result = await _controller.AboutToAskForClarification(applicationId, confirmAskForClarification);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as RoatpGatewayClarificationsViewModel;
            Assert.That(viewResult.ViewName.Contains("AskForClarification.cshtml"), Is.True);
            Assert.That(1, Is.EqualTo(model.ErrorMessages.Count));
            Assert.That("ConfirmAskForClarification", Is.EqualTo(model.ErrorMessages[0].Field));
        }

        [Test]
        public async Task AboutToAskForClarification_with_no_then_redirected_to_applications_page()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync(new RoatpGatewayClarificationsViewModel());

            var confirmAskForClarification = "No";
            var result = await _controller.AboutToAskForClarification(applicationId, confirmAskForClarification);

            var viewResult = result as RedirectToActionResult;
            Assert.That("ViewApplication", Is.EqualTo(viewResult.ActionName));
        }


        [Test]
        public async Task AboutToAskForClarification_with_yes_then_redirected_to_successful_page()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync(new RoatpGatewayClarificationsViewModel());

            ApplyApiClient.Setup(x => x.UpdateGatewayReviewStatusAsClarification(applicationId, It.IsAny<string>(), It.IsAny<string>()));

            var confirmAskForClarification = "Yes";
            var result = await _controller.AboutToAskForClarification(applicationId, confirmAskForClarification);
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewName.Contains("ConfirmApplicationClarification.cshtml"), Is.True);
            ApplyApiClient.Verify(x => x.UpdateGatewayReviewStatusAsClarification(applicationId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AboutToAskForClarification_with_ClarificationSent_then_redirected_to_applications_page()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetClarificationViewModel(It.IsAny<GetApplicationClarificationsRequest>()))
                .ReturnsAsync(new RoatpGatewayClarificationsViewModel { GatewayReviewStatus = GatewayReviewStatus.ClarificationSent });

            var confirmAskForClarification = "Yes";
            var result = await _controller.AboutToAskForClarification(applicationId, confirmAskForClarification);

            var viewResult = result as RedirectToActionResult;
            Assert.That("ViewApplication", Is.EqualTo(viewResult.ActionName));
        }

        [TestCase(GatewayReviewStatus.New)]
        [TestCase(GatewayReviewStatus.InProgress)]
        [TestCase(GatewayReviewStatus.ClarificationSent)]
        public async Task ViewApplication_when_submitted_for_review_shows_expected_view(string gatewayReviewStatus)
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplicationStatus = ApplicationStatus.Submitted,
                GatewayReviewStatus = gatewayReviewStatus,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var oversightDetails = new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None };

            var viewmodel = new RoatpGatewayApplicationViewModel(application, oversightDetails);
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(viewmodel);

            var result = await _controller.ViewApplication(applicationId);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("Application.cshtml"), Is.True);
        }

        [TestCase(GatewayReviewStatus.Pass)]
        [TestCase(GatewayReviewStatus.Fail)]
        [TestCase(GatewayReviewStatus.Rejected)]
        public async Task ViewApplication_when_gateway_assessed_shows_expected_view(string gatewayReviewStatus)
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplicationStatus = ApplicationStatus.GatewayAssessed,
                GatewayReviewStatus = gatewayReviewStatus,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var viewmodel = new RoatpGatewayApplicationViewModel(application, new ApplicationOversightDetails());
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(viewmodel);

            var result = await _controller.ViewApplication(applicationId);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("Application_ReadOnly.cshtml"), Is.True);
        }

        [Test]
        public async Task ViewApplication_when_Application_Withdrawn_shows_expected_view()
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplicationStatus = ApplicationStatus.Withdrawn,
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var viewmodel = new RoatpGatewayApplicationViewModel(application, new ApplicationOversightDetails());
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(viewmodel);

            var result = await _controller.ViewApplication(applicationId);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("Application_Closed.cshtml"), Is.True);
        }

        [Test]
        public async Task ViewApplication_when_Application_Removed_shows_expected_view()
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplicationStatus = ApplicationStatus.Removed,
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var viewmodel = new RoatpGatewayApplicationViewModel(application, new ApplicationOversightDetails());
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(viewmodel);

            var result = await _controller.ViewApplication(applicationId);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("Application_Closed.cshtml"), Is.True);
        }
    }
}
