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
    public class RoatpGatewayApplicationActionsControllerTests : RoatpGatewayControllerTestBase<RoatpGatewayController>
    {
        private RoatpGatewayApplicationActionsController _controller;
        private Mock<IGatewayApplicationActionsOrchestrator> _applicationActionsOrchestrator;
        private Mock<IRoatpRemoveApplicationViewModelValidator> _removeApplicationValidator;
        private Mock<IRoatpWithdrawApplicationViewModelValidator> _withdrawApplicationValidator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _applicationActionsOrchestrator = new Mock<IGatewayApplicationActionsOrchestrator>();
            _removeApplicationValidator = new Mock<IRoatpRemoveApplicationViewModelValidator>();
            _withdrawApplicationValidator = new Mock<IRoatpWithdrawApplicationViewModelValidator>();

            _controller = new RoatpGatewayApplicationActionsController(ApplyApiClient.Object, _applicationActionsOrchestrator.Object,
                                                                        _removeApplicationValidator.Object, _withdrawApplicationValidator.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task RemoveApplication_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpRemoveApplicationViewModel { ApplicationId = applicationId };

            _applicationActionsOrchestrator.Setup(x => x.GetRemoveApplicationViewModel(applicationId, UserId, Username)).ReturnsAsync(expectedViewModel);

            var result = await _controller.RemoveApplication(applicationId);
            var viewResult = result as ViewResult;
            Assert.That(expectedViewModel, Is.SameAs(viewResult.Model));
        }

        [Test]
        public async Task ConfirmRemoveApplication_No_selected_returns_to_ViewApplication()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });

            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });

            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.That(nameof(RoatpGatewayController.ViewApplication), Is.EqualTo(viewResult.ActionName));
            Assert.That("RoatpGateway", Is.EqualTo(viewResult.ControllerName));
        }

        [Test]
        public async Task ConfirmRemoveApplication_When_oversight_performed_selected_returns_to_ViewApplication()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.Successful });
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.That(nameof(RoatpGatewayController.ViewApplication), Is.EqualTo(viewResult.ActionName));
            Assert.That("RoatpGateway", Is.EqualTo(viewResult.ControllerName));
        }

        [Test]
        public async Task ConfirmRemoveApplication_Yes_selected_And_fails_validation_returns_back_to_View()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "Yes"
            };

            var validationErrors = new List<ValidationErrorDetail> { new ValidationErrorDetail() };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("ConfirmRemoveApplication.cshtml"), Is.True);
        }

        [Test]
        [Ignore("This test is not yet ready as functionality is not implemented")]
        public async Task ConfirmRemoveApplication_Yes_selected_And_passed_validation_performs_Application_Removal()
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes
            };

            var validationErrors = new List<ValidationErrorDetail>();

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(application);
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("ApplicationRemoved.cshtml"), Is.True);
            ApplyApiClient.Verify(x => x.RemoveApplication(viewModel.ApplicationId, viewModel.OptionYesText, viewModel.OptionYesTextExternal, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public async Task WithdrawApplication_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpWithdrawApplicationViewModel { ApplicationId = applicationId };

            _applicationActionsOrchestrator.Setup(x => x.GetWithdrawApplicationViewModel(applicationId, Username)).ReturnsAsync(expectedViewModel);

            var result = await _controller.WithdrawApplication(applicationId);
            var viewResult = result as ViewResult;
            Assert.That(expectedViewModel, Is.SameAs(viewResult.Model));
        }

        [Test]
        public async Task ConfirmWithdrawApplication_No_selected_returns_to_ViewApplication()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.That(nameof(RoatpGatewayController.ViewApplication), Is.EqualTo(viewResult.ActionName));
            Assert.That("RoatpGateway", Is.EqualTo(viewResult.ControllerName));
        }

        [TestCase(OversightReviewStatus.Successful)]
        [TestCase(OversightReviewStatus.Unsuccessful)]
        public async Task ConfirmWithdrawApplication_When_oversight_performed_returns_to_ViewApplication(OversightReviewStatus oversightStatus)
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = oversightStatus });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.That(nameof(RoatpGatewayController.ViewApplication), Is.EqualTo(viewResult.ActionName));
            Assert.That("RoatpGateway", Is.EqualTo(viewResult.ControllerName));
        }

        public async Task ConfirmWithdrawApplication_When_already_withdrawn_returns_to_ViewApplication(string oversightStatus)
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueNo,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId, ApplicationStatus = ApplicationStatus.Withdrawn });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.That(nameof(RoatpGatewayController.ViewApplication), Is.EqualTo(viewResult.ActionName));
            Assert.That("RoatpGateway", Is.EqualTo(viewResult.ControllerName));
        }

        [Test]
        public async Task ConfirmWithdrawApplication_Yes_selected_And_fails_validation_returns_back_to_View()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes
            };

            var validationErrors = new List<ValidationErrorDetail> { new ValidationErrorDetail() };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("ConfirmWithdrawApplication.cshtml"), Is.True);
        }

        [Test]
        public async Task ConfirmWithdrawApplication_Yes_selected_And_passed_validation_performs_Application_Withdrawal()
        {
            var applicationId = Guid.NewGuid();

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplyData = new ApplyData { ApplyDetails = new ApplyDetails() }
            };

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = HtmlAndCssElements.RadioButtonValueYes,
                OptionYesText = "Comments"
            };

            var validationErrors = new List<ValidationErrorDetail>();

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(application);
            ApplyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith("ApplicationWithdrawn.cshtml"), Is.True);
            ApplyApiClient.Verify(x => x.WithdrawApplication(viewModel.ApplicationId, viewModel.OptionYesText, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
