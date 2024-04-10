using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayOverviewOrchestratorTests
    {
        private GatewayOverviewOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IGatewaySectionsNotRequiredService> _sectionsNotRequiredService;

        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _sectionsNotRequiredService = new Mock<IGatewaySectionsNotRequiredService>();
            _orchestrator = new GatewayOverviewOrchestrator(_applyApiClient.Object, _sectionsNotRequiredService.Object);
        }

        [TestCase("12345678", "John Ltd.", SectionReviewStatus.Pass, "Very good.", false)]
        [TestCase("87654321", "Simon Ltd.", SectionReviewStatus.Fail, "Not so good.", false)]
        [TestCase("43211234", "Bob Ltd.", SectionReviewStatus.Clarification, "Needs clarification.", true)]
        [TestCase("12344321", "Frank Ltd.", SectionReviewStatus.NotRequired, null, false)]
        public async Task GetConfirmOverviewViewModel_returns_model(string ukprn, string organisationName, string sectionReviewStatus, string comment, bool isClarificationSet)
        {
            var applicationId = Guid.NewGuid();
            var gatewayReviewStatus = GatewayReviewStatus.InProgress;

            var applyData = new ApplyData
            {
                ApplyDetails = new ApplyDetails
                {
                    UKPRN = ukprn,
                    OrganisationName = organisationName
                }
            };

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            _applyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(application);
            _applyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });

            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>
            {
                new GatewayPageAnswerSummary
                {
                    ApplicationId = applicationId,
                    PageId = GatewayPageIds.OrganisationRisk,
                    Status = sectionReviewStatus,
                    Comments = comment
                }
            };

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var request = new GetApplicationOverviewRequest(applicationId, UserName);

            var viewModel = await _orchestrator.GetConfirmOverviewViewModel(request);

            Assert.That(applicationId, Is.EqualTo(viewModel.ApplicationId));
            Assert.That(ukprn, Is.EqualTo(viewModel.Ukprn));
            Assert.That(organisationName, Is.EqualTo(viewModel.OrganisationName));
            Assert.That(gatewayReviewStatus, Is.EqualTo(viewModel.GatewayReviewStatus));
            Assert.That(sectionReviewStatus, Is.EqualTo(viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Status));
            Assert.That(comment, Is.EqualTo(viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Comment));
            Assert.That(viewModel.IsClarificationsSelectedAndAllFieldsSet, Is.EqualTo(isClarificationSet));
        }

        [TestCase("12345678", "John Ltd.")]
        [TestCase("87654321", "Simon Ltd.")]
        [TestCase("12344321", "Frank Ltd.")]
        public async Task GetConfirmOverviewViewModel_returns_model_no_saved_statuses(string ukprn, string organisationName)
        {
            var applicationId = Guid.NewGuid();
            var gatewayReviewStatus = GatewayReviewStatus.InProgress;

            var applyData = new ApplyData
            {
                ApplyDetails = new ApplyDetails
                {
                    UKPRN = ukprn,
                    OrganisationName = organisationName
                }
            };

            var application = new Apply
            {
                ApplicationId = applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            _applyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(application);
            _applyApiClient.Setup(x => x.GetOversightDetails(applicationId)).ReturnsAsync(() =>
                new ApplicationOversightDetails { OversightStatus = OversightReviewStatus.None });

            // No Saved Statuses
            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>();

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var request = new GetApplicationOverviewRequest(applicationId, UserName);

            var viewModel = await _orchestrator.GetConfirmOverviewViewModel(request);

            viewModel.ApplicationId.Should().Be(applicationId);
            viewModel.Ukprn.Should().Be(ukprn);
            viewModel.OrganisationName.Should().Be(organisationName);
            viewModel.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModel.ReadyToConfirm.Should().Be(false);
        }

        private RoatpGatewayApplicationViewModel ProcessViewModelOnError(Guid applicationId, string gatewayReviewStatus, string field, string errorMessage)
        {
            var viewModelOnError = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = gatewayReviewStatus
            };

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = gatewayReviewStatus
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

            _orchestrator.ProcessViewModelOnError(viewModelOnError, viewModel, validationResponse);

            return viewModelOnError;
        }

        [TestCase(GatewayReviewStatus.InProgress, "GatewayReviewStatus", "Error - GatewayReviewStatus")]
        public void ProcessViewModelOnError_process_view_model_correctly_GatewayReviewStatus(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextGatewayReviewStatus.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.ClarificationSent, "OptionAskClarificationText", "Error - Ask for Clarification")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionAskClarificationText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextAskClarification.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.Fail, "OptionFailedText", "Error - Decline")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionFailedText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextFailed.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.Rejected, "OptionRejectedText", "Error - Decline")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionRejectedText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextRejected.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.Pass, "OptionApprovedText", "Error - Pass")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionApprovedText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextApproved.Should().Be(errorMessage);
        }
    }
}
