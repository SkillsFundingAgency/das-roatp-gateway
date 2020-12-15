using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Web.Controllers;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpGateway.Domain.Apply;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpGatewayControllerTests_ApplicationActions : RoatpGatewayControllerTestBase<RoatpGatewayController>
    {
        private RoatpGatewayController _controller;
        private Mock<IGatewayOverviewOrchestrator> _orchestrator;
        private Mock<IRoatpGatewayApplicationViewModelValidator> _validator;
        private Mock<IRoatpGatewayPageValidator> _pageValidator;
        private Mock<IGatewayApplicationActionsOrchestrator> _applicationActionsOrchestrator;
        private Mock<IRoatpRemoveApplicationViewModelValidator> _removeApplicationValidator;
        private Mock<IRoatpWithdrawApplicationViewModelValidator> _withdrawApplicationValidator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOverviewOrchestrator>();
            _validator = new Mock<IRoatpGatewayApplicationViewModelValidator>();
            _pageValidator = new Mock<IRoatpGatewayPageValidator>();
            _applicationActionsOrchestrator = new Mock<IGatewayApplicationActionsOrchestrator>();
            _removeApplicationValidator = new Mock<IRoatpRemoveApplicationViewModelValidator>();
            _withdrawApplicationValidator = new Mock<IRoatpWithdrawApplicationViewModelValidator>();

            _controller = new RoatpGatewayController(ApplyApiClient.Object, _orchestrator.Object,
                                                     _validator.Object, _applicationActionsOrchestrator.Object,
                                                     _removeApplicationValidator.Object, _withdrawApplicationValidator.Object,
                                                     Logger.Object, _pageValidator.Object);

            _controller.ControllerContext = MockedControllerContext.Setup();
            UserId = _controller.ControllerContext.HttpContext.User.UserId();
            Username = _controller.ControllerContext.HttpContext.User.UserDisplayName();
        }

        [Test]
        public async Task RemoveApplication_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpRemoveApplicationViewModel { ApplicationId = applicationId };

            _applicationActionsOrchestrator.Setup(x => x.GetRemoveApplicationViewModel(applicationId, Username)).ReturnsAsync(expectedViewModel);

            var result = await _controller.RemoveApplication(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModel, viewResult.Model);
        }

        [Test]
        public async Task ConfirmRemoveApplication_No_selected_returns_to_ViewApplication()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "No"
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.AreEqual(nameof(_controller.ViewApplication), viewResult.ActionName);
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
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.IsTrue(viewResult.ViewName.EndsWith("ConfirmRemoveApplication.cshtml"));
        }

        [Test]
        [Ignore("This test is not yet ready as functionality is not implemented")]
        public async Task ConfirmRemoveApplication_Yes_selected_And_passed_validation_performs_Application_Removal()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "Yes"
            };

            var validationErrors = new List<ValidationErrorDetail>();

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _removeApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmRemoveApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.Inconclusive();
        }


        [Test]
        public async Task WithdrawApplication_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpWithdrawApplicationViewModel { ApplicationId = applicationId };

            _applicationActionsOrchestrator.Setup(x => x.GetWithdrawApplicationViewModel(applicationId, Username)).ReturnsAsync(expectedViewModel);

            var result = await _controller.WithdrawApplication(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModel, viewResult.Model);
        }

        [Test]
        public async Task ConfirmWithdrawApplication_No_selected_returns_to_ViewApplication()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "No",
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as RedirectToActionResult;

            Assert.AreEqual(nameof(_controller.ViewApplication), viewResult.ActionName);
        }

        [Test]
        public async Task ConfirmWithdrawApplication_Yes_selected_And_fails_validation_returns_back_to_View()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "Yes"
            };

            var validationErrors = new List<ValidationErrorDetail> { new ValidationErrorDetail() };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.IsTrue(viewResult.ViewName.EndsWith("ConfirmWithdrawApplication.cshtml"));
        }

        [Test]
        [Ignore("This test is not yet ready as functionality is not implemented")]
        public async Task ConfirmWithdrawApplication_Yes_selected_And_passed_validation_performs_Application_Withdrawal()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
                ConfirmApplicationAction = "Yes"
            };

            var validationErrors = new List<ValidationErrorDetail>();

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _withdrawApplicationValidator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = validationErrors });

            var result = await _controller.ConfirmWithdrawApplication(applicationId, viewModel);
            var viewResult = result as ViewResult;

            Assert.Inconclusive();
        }
    }
}
