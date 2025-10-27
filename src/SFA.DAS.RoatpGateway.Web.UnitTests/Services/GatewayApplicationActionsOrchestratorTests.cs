using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

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

            Assert.That(viewModel.ApplicationId, Is.EqualTo(_applicationId));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
            Assert.That(viewModel.ApplicationSubmittedOn, Is.EqualTo(submittedDate));
            Assert.That(viewModel.ApplyLegalName, Is.EqualTo(organisationName));
            Assert.That(viewModel.ApplicationRoute, Is.EqualTo(providerRouteName));
            Assert.That(viewModel.ApplicationStatus, Is.EqualTo(applicationStatus));
            Assert.That(viewModel.ApplicationReferenceNumber, Is.EqualTo(referenceNumber));
            Assert.That(viewModel.ApplicationEmailAddress, Is.EqualTo(email));
        }

        [Test]
        public void ProcessWithdrawApplicationViewModelOnError_process_view_model_correctly_for_ConfirmApplicationActionYes()
        {
            var applicationId = Guid.NewGuid();
            var field = "ConfirmApplicationActionYes";
            var errorMessage = "Error";

            var viewModelOnError = ProcessWithdrawApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.That(viewModelOnError.ApplicationId, Is.EqualTo(applicationId));
            Assert.That(viewModelOnError.ErrorMessages, Is.Not.Null);
            Assert.That(field, Is.EqualTo(viewModelOnError.ErrorMessages[0].Field));
            Assert.That(errorMessage, Is.EqualTo(viewModelOnError.ErrorMessages[0].ErrorMessage));
            Assert.That(viewModelOnError.CssFormGroupError, Is.EqualTo(HtmlAndCssElements.CssFormGroupErrorClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesText, Is.Null);
        }

        [Test]
        public void ProcessWithdrawApplicationViewModelOnError_process_view_model_correctly_for_OptionYesText()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpWithdrawApplicationViewModel.OptionYesText);
            var errorMessage = "Error";

            var viewModelOnError = ProcessWithdrawApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.That(viewModelOnError.ApplicationId, Is.EqualTo(applicationId));
            Assert.That(viewModelOnError.ErrorMessages, Is.Not.Null);
            Assert.That(field, Is.EqualTo(viewModelOnError.ErrorMessages[0].Field));
            Assert.That(errorMessage, Is.EqualTo(viewModelOnError.ErrorMessages[0].ErrorMessage));
            Assert.That(viewModelOnError.CssFormGroupError, Is.EqualTo(HtmlAndCssElements.CssFormGroupErrorClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesText, Is.EqualTo(HtmlAndCssElements.CssTextareaErrorOverrideClass));
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

            Assert.That(viewModel.ApplicationId, Is.EqualTo(_applicationId));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
            Assert.That(viewModel.ApplicationSubmittedOn, Is.EqualTo(submittedDate));
            Assert.That(viewModel.ApplyLegalName, Is.EqualTo(organisationName));
            Assert.That(viewModel.ApplicationRoute, Is.EqualTo(providerRouteName));
            Assert.That(viewModel.ApplicationStatus, Is.EqualTo(applicationStatus));
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_ConfirmApplicationActionYes()
        {
            var applicationId = Guid.NewGuid();
            var field = "ConfirmApplicationActionYes";
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.That(viewModelOnError.ApplicationId, Is.EqualTo(applicationId));
            Assert.That(viewModelOnError.ErrorMessages, Is.Not.Null);
            Assert.That(field, Is.EqualTo(viewModelOnError.ErrorMessages[0].Field));
            Assert.That(errorMessage, Is.EqualTo(viewModelOnError.ErrorMessages[0].ErrorMessage));
            Assert.That(viewModelOnError.CssFormGroupError, Is.EqualTo(HtmlAndCssElements.CssFormGroupErrorClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesText, Is.Null);
            Assert.That(viewModelOnError.CssOnErrorOptionYesTextExternal, Is.Null);
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_OptionYesText()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpRemoveApplicationViewModel.OptionYesText);
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.That(viewModelOnError.ApplicationId, Is.EqualTo(applicationId));
            Assert.That(viewModelOnError.ErrorMessages, Is.Not.Null);
            Assert.That(field, Is.EqualTo(viewModelOnError.ErrorMessages[0].Field));
            Assert.That(errorMessage, Is.EqualTo(viewModelOnError.ErrorMessages[0].ErrorMessage));
            Assert.That(viewModelOnError.CssFormGroupError, Is.EqualTo(HtmlAndCssElements.CssFormGroupErrorClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesText, Is.EqualTo(HtmlAndCssElements.CssTextareaErrorOverrideClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesTextExternal, Is.Null);
        }

        [Test]
        public void ProcessRemoveApplicationViewModelOnError_process_view_model_correctly_for_OptionYesTextExternal()
        {
            var applicationId = Guid.NewGuid();
            var field = nameof(RoatpRemoveApplicationViewModel.OptionYesTextExternal);
            var errorMessage = "Error";

            var viewModelOnError = ProcessRemoveApplicationViewModelOnError(applicationId, field, errorMessage);

            Assert.That(viewModelOnError.ApplicationId, Is.EqualTo(applicationId));
            Assert.That(viewModelOnError.ErrorMessages, Is.Not.Null);
            Assert.That(field, Is.EqualTo(viewModelOnError.ErrorMessages[0].Field));
            Assert.That(errorMessage, Is.EqualTo(viewModelOnError.ErrorMessages[0].ErrorMessage));
            Assert.That(viewModelOnError.CssFormGroupError, Is.EqualTo(HtmlAndCssElements.CssFormGroupErrorClass));
            Assert.That(viewModelOnError.CssOnErrorOptionYesText, Is.Null);
            Assert.That(viewModelOnError.CssOnErrorOptionYesTextExternal, Is.EqualTo(HtmlAndCssElements.CssTextareaErrorOverrideClass));
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
