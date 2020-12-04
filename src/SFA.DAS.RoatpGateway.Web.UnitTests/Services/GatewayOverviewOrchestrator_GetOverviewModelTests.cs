using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayOverviewOrchestratorGetOverviewModelTests
    {
        private GatewayOverviewOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IGatewaySectionsNotRequiredService> _sectionsNotRequiredService;
        private readonly Guid _applicationId = Guid.NewGuid();
        private const string UserName = "GatewayUser";
        private const string Ukprn = "12345678";
        private const string OrganisationName = "Mark's cafe";
        private const string Email = "mark@test.com";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _sectionsNotRequiredService = new Mock<IGatewaySectionsNotRequiredService>();
            _orchestrator = new GatewayOverviewOrchestrator(_applyApiClient.Object, _sectionsNotRequiredService.Object);
        }

        [Test]
        public async Task GetOverviewViewModel_returns_model()
        {
            var applyData = new RoatpApplyData
            {
                ApplyDetails = new RoatpApplyDetails
                {
                    UKPRN = Ukprn,
                    OrganisationName = OrganisationName
                }
            };

            var returnedRoatpApplicationResponse = new RoatpApplicationResponse
            {
                ApplicationId = _applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            var contactDetails = new ContactDetails {Email = Email};
            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(returnedRoatpApplicationResponse);
            _applyApiClient.Setup(x => x.GetContactDetails(_applicationId)).ReturnsAsync(contactDetails);
            const string sectionReviewStatus = SectionReviewStatus.Pass;
            const string comment = "comments go here";

            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>
            {
                new GatewayPageAnswerSummary
                {
                    ApplicationId = _applicationId,
                    PageId = GatewayPageIds.OrganisationRisk,
                    Status = sectionReviewStatus,
                    Comments = comment
                }
            };

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(_applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var gatewayReviewStatus = GatewayReviewStatus.InProgress;
            var request = new GetApplicationOverviewRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetOverviewViewModel(request);

            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(viewModel.ApplicationEmailAddress,Email);
            Assert.AreEqual(Ukprn, viewModel.Ukprn);
            Assert.AreEqual(OrganisationName, viewModel.OrganisationName);
            Assert.AreEqual(gatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(sectionReviewStatus, viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Status);
            Assert.AreEqual(false, viewModel.AreClarificationsSelected);
        }





        [Test]
        public async Task GetOverviewViewModel_with_clarifications_returns_model()
        {
            var applyData = new RoatpApplyData
            {
                ApplyDetails = new RoatpApplyDetails
                {
                    UKPRN = Ukprn,
                    OrganisationName = OrganisationName
                }
            };

            var returnedRoatpApplicationResponse = new RoatpApplicationResponse
            {
                ApplicationId = _applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            var contactDetails = new ContactDetails { Email = Email };
            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(returnedRoatpApplicationResponse);
            _applyApiClient.Setup(x => x.GetContactDetails(_applicationId)).ReturnsAsync(contactDetails);
            const string sectionReviewStatus = SectionReviewStatus.Clarification;
            const string comment = "comments go here";

            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>
            {
                new GatewayPageAnswerSummary
                {
                    ApplicationId = _applicationId,
                    PageId = GatewayPageIds.OrganisationRisk,
                    Status = sectionReviewStatus,
                    Comments = comment
                }
            };

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(_applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var gatewayReviewStatus = GatewayReviewStatus.InProgress;
            var request = new GetApplicationOverviewRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetOverviewViewModel(request);

            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(viewModel.ApplicationEmailAddress, Email);
            Assert.AreEqual(Ukprn, viewModel.Ukprn);
            Assert.AreEqual(OrganisationName, viewModel.OrganisationName);
            Assert.AreEqual(gatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(sectionReviewStatus, viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Status);
            Assert.AreEqual(true, viewModel.AreClarificationsSelected);
            Assert.AreEqual(false, viewModel.ReadyToConfirm);
        }
    }
}
