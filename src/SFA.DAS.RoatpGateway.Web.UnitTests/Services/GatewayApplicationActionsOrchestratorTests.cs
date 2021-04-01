using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayApplicationActionsOrchestratorTests
    {
        private GatewayApplicationActionsOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;

        private Guid _applicationId;
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";

        [SetUp]
        public void Setup()
        {
            _applicationId = Guid.NewGuid();
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new GatewayApplicationActionsOrchestrator(_applyApiClient.Object);
        }

        [TestCase("12345678", "John Ltd.", "Main", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.Pass)]
        [TestCase("87654321", "Simon Ltd.", "Employer", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.Fail)]
        [TestCase("43211234", "Bob Ltd.", "Supporting", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.ClarificationSent)]
        public async Task GetWithdrawApplicationViewModel_returns_viewModel(string ukprn, string organisationName, string providerRouteName, string applicationStatus, string gatewayReviewStatus)
        {
            var submittedDate = DateTime.Now.AddDays(-1);
            var referenceNumber = "APR100012";
            var email = "test@test.com";

            var apply = new Apply
            {
                ApplicationId = _applicationId,
                ApplicationStatus = applicationStatus,
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails
                    {
                        UKPRN = ukprn,
                        OrganisationName = organisationName,
                        ProviderRouteName = providerRouteName,
                        ReferenceNumber = referenceNumber,
                        ApplicationSubmittedOn = submittedDate
                    }
                }
            };

            var contact = new ContactDetails
            {
                Email = email
            };

            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(apply);
            _applyApiClient.Setup(x => x.GetContactDetails(_applicationId)).ReturnsAsync(contact);

            var viewModel = await _orchestrator.GetWithdrawApplicationViewModel(_applicationId, UserName);

            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(submittedDate, viewModel.ApplicationSubmittedOn);
            Assert.AreEqual(organisationName, viewModel.ApplyLegalName);
            Assert.AreEqual(providerRouteName, viewModel.ApplicationRoute);
            Assert.AreEqual(applicationStatus, viewModel.ApplicationStatus);
            Assert.AreEqual(referenceNumber, viewModel.ApplicationReferenceNumber);
            Assert.AreEqual(email, viewModel.ApplicationEmailAddress);
        }

        [Test]
        public void ProcessWithdrawApplicationViewModelOnError_process_view_model_correctly_for_ConfirmApplicationActionYes()
        {
            var applicationId = Guid.NewGuid();
            var field = "ConfirmApplicationActionYes";
            var errorMessage = "Error";

            var viewModelOnError = ProcessWithdrawApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.AreEqual(applicationId, viewModelOnError.ApplicationId);
            Assert.IsNotNull(viewModelOnError.ErrorMessages);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].Field, field);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].ErrorMessage, errorMessage);
            Assert.AreEqual(HtmlAndCssElements.CssFormGroupErrorClass, viewModelOnError.CssFormGroupError);
            Assert.IsNull( viewModelOnError.CssOnErrorOptionYesText);
        }

        [Test]
        public void ProcessWithdrawApplicationViewModelOnError_process_view_model_correctly_for_OptionYesText()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpWithdrawApplicationViewModel.OptionYesText);
            var errorMessage = "Error";

            var viewModelOnError = ProcessWithdrawApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.AreEqual(applicationId, viewModelOnError.ApplicationId);
            Assert.IsNotNull(viewModelOnError.ErrorMessages);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].Field, field);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].ErrorMessage, errorMessage);
            Assert.AreEqual(HtmlAndCssElements.CssFormGroupErrorClass, viewModelOnError.CssFormGroupError);
            Assert.AreEqual(HtmlAndCssElements.CssTextareaErrorOverrideClass, viewModelOnError.CssOnErrorOptionYesText);
        }

        private RoatpWithdrawApplicationViewModel ProcessWithdrawApplicationViewModelOnError(Guid applicationId, string field, string errorMessage)
        {
            var viewModelOnError = new RoatpWithdrawApplicationViewModel
            {
                ApplicationId = applicationId,
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Field = field,
                        ErrorMessage = errorMessage
                    }
                }
            };

            _orchestrator.ProcessWithdrawApplicationViewModelOnError(viewModelOnError, validationResponse);

            return viewModelOnError;
        }


        [TestCase("12345678", "John Ltd.", "Main", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.Pass)]
        [TestCase("87654321", "Simon Ltd.", "Employer", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.Fail)]
        [TestCase("43211234", "Bob Ltd.", "Supporting", ApplicationStatus.GatewayAssessed, GatewayReviewStatus.ClarificationSent)]
        public async Task GetRemoveApplicationViewModel_returns_viewModel(string ukprn, string organisationName, string providerRouteName, string applicationStatus, string gatewayReviewStatus)
        {
            var submittedDate = DateTime.Now.AddDays(-1);

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationId = _applicationId,
                Ukprn = ukprn,
                ApplicationSubmittedOn = submittedDate,
                LegalName = organisationName,
                ProviderRouteName = providerRouteName,
                ApplicationStatus = applicationStatus,
                GatewayReviewStatus = gatewayReviewStatus
            };

            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, It.IsAny<string>(), UserId, UserName)).ReturnsAsync(commonDetails);

            var viewModel = await _orchestrator.GetRemoveApplicationViewModel(_applicationId, UserId, UserName);

            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(submittedDate, viewModel.ApplicationSubmittedOn);
            Assert.AreEqual(organisationName, viewModel.ApplyLegalName);
            Assert.AreEqual(providerRouteName, viewModel.ApplicationRoute);
            Assert.AreEqual(applicationStatus, viewModel.ApplicationStatus);
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_ConfirmApplicationActionYes()
        {
            var applicationId = Guid.NewGuid();
            var field = "ConfirmApplicationActionYes";
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.AreEqual(applicationId, viewModelOnError.ApplicationId);
            Assert.IsNotNull(viewModelOnError.ErrorMessages);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].Field, field);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].ErrorMessage, errorMessage);
            Assert.AreEqual(HtmlAndCssElements.CssFormGroupErrorClass, viewModelOnError.CssFormGroupError);
            Assert.IsNull(viewModelOnError.CssOnErrorOptionYesText);
            Assert.IsNull(viewModelOnError.CssOnErrorOptionYesTextExternal);
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_OptionYesText()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpRemoveApplicationViewModel.OptionYesText);
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.AreEqual(applicationId, viewModelOnError.ApplicationId);
            Assert.IsNotNull(viewModelOnError.ErrorMessages);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].Field, field);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].ErrorMessage, errorMessage);
            Assert.AreEqual(HtmlAndCssElements.CssFormGroupErrorClass, viewModelOnError.CssFormGroupError);
            Assert.AreEqual(HtmlAndCssElements.CssTextareaErrorOverrideClass, viewModelOnError.CssOnErrorOptionYesText);
            Assert.IsNull(viewModelOnError.CssOnErrorOptionYesTextExternal);
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_OptionYesTextExternal()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpRemoveApplicationViewModel.OptionYesTextExternal);
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.AreEqual(applicationId, viewModelOnError.ApplicationId);
            Assert.IsNotNull(viewModelOnError.ErrorMessages);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].Field, field);
            Assert.AreEqual(viewModelOnError.ErrorMessages[0].ErrorMessage, errorMessage);
            Assert.AreEqual(HtmlAndCssElements.CssFormGroupErrorClass, viewModelOnError.CssFormGroupError);
            Assert.IsNull(viewModelOnError.CssOnErrorOptionYesText);
            Assert.AreEqual(HtmlAndCssElements.CssTextareaErrorOverrideClass, viewModelOnError.CssOnErrorOptionYesTextExternal);
        }

        private RoatpRemoveApplicationViewModel ProcessRemoveApplicationViewModelOnError(Guid applicationId, string field, string errorMessage)
        {
            var viewModelOnError = new RoatpRemoveApplicationViewModel
            {
                ApplicationId = applicationId,
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Field = field,
                        ErrorMessage = errorMessage
                    }
                }
            };

            _orchestrator.ProcessRemoveApplicationViewModelOnError(viewModelOnError, validationResponse);

            return viewModelOnError;
        }
    }
}
