using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayCriminalComplianceChecksOrchestratorTests
    {
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpGatewayCriminalComplianceChecksApiClient> _criminalChecksApiClient;
        private Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>> _logger;
        private GatewayCriminalComplianceChecksOrchestrator _orchestrator;

        private GatewayCommonDetails _commonDetails;

        [SetUp]
        public void Before_each_test()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _criminalChecksApiClient = new Mock<IRoatpGatewayCriminalComplianceChecksApiClient>();
            _logger = new Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>>();

            _orchestrator = new GatewayCriminalComplianceChecksOrchestrator(_applyApiClient.Object, _criminalChecksApiClient.Object, _logger.Object);

            _commonDetails = new GatewayCommonDetails
            {
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                LegalName = "Legal Eagle",
                Status = "Pass",
                Comments = "No comment",
                Ukprn = "10001234",
                SourcesCheckedOn = DateTime.Now.AddHours(-2),
                ApplicationSubmittedOn = DateTime.Now.AddMonths(-1),
                OutcomeMadeBy = "Test",
                OutcomeMadeOn = DateTime.Now
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_commonDetails);
        }

        [Test]
        public async Task Orchestrator_when_Organisation_Page_builds_view_model_from_api()
        {
            var pageId = GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors;
            var questionId = "CC-20";
            var furtherQuestionId = "CC-20_1";

            _commonDetails.PageId = pageId;

            var criminalComplianceDetails = new CriminalComplianceCheckDetails
            {
                QuestionText = "What is the question",
                Answer = "Yes",
                QuestionId = questionId,
                PageId = pageId,
                FurtherAnswer = "Lorem ipsum",
                FurtherQuestionId = furtherQuestionId
            };

            _criminalChecksApiClient.Setup(x => x.GetCriminalComplianceQuestionDetails(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(criminalComplianceDetails);

            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), pageId, "userId", "user"));

            viewModel.PageId.Should().Be(pageId);
            viewModel.Caption.Should().Be(RoatpGatewayConstants.Captions.OrganisationsCriminalAndComplianceChecks);
            viewModel.QuestionText.Should().Be(criminalComplianceDetails.QuestionText);
            viewModel.Ukprn.Should().Be(_commonDetails.Ukprn);
            viewModel.ApplyLegalName.Should().Be(_commonDetails.LegalName);
            viewModel.ComplianceCheckQuestionId.Should().Be(questionId);
            viewModel.ComplianceCheckAnswer.Should().Be("Yes");
            viewModel.FurtherInformationQuestionId.Should().Be(furtherQuestionId);
            viewModel.FurtherInformationAnswer.Should().Be("Lorem ipsum");
        }

        [Test]
        public async Task Orchestrator_when_PeopleInControl_Page_builds_view_model_from_api()
        {
            var pageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions;
            var questionId = "CC-40";
            var furtherQuestionId = "CC-40_1";

            _commonDetails.PageId = pageId;

            var criminalComplianceDetails = new CriminalComplianceCheckDetails
            {
                QuestionText = "What is the question",
                Answer = "Yes",
                QuestionId = questionId,
                PageId = pageId,
                FurtherAnswer = "Lorem ipsum",
                FurtherQuestionId = furtherQuestionId
            };

            _criminalChecksApiClient.Setup(x => x.GetCriminalComplianceQuestionDetails(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(criminalComplianceDetails);

            var viewModel = await _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), pageId, "userId", "user"));

            viewModel.PageId.Should().Be(pageId);
            viewModel.Caption.Should().Be(RoatpGatewayConstants.Captions.PeopleInControlCriminalAndComplianceChecks);
            viewModel.QuestionText.Should().Be(criminalComplianceDetails.QuestionText);
            viewModel.Ukprn.Should().Be(_commonDetails.Ukprn);
            viewModel.ApplyLegalName.Should().Be(_commonDetails.LegalName);
            viewModel.ComplianceCheckQuestionId.Should().Be(questionId);
            viewModel.ComplianceCheckAnswer.Should().Be("Yes");
            viewModel.FurtherInformationQuestionId.Should().Be(furtherQuestionId);
            viewModel.FurtherInformationAnswer.Should().Be("Lorem ipsum");
        }
    }
}
