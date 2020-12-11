using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayOverviewOrchestratorGetClarificationViewModelTests
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
        public async Task GetClarificationViewModel_returns_null_if_no_application()
        {
            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync((Apply)null);
            var request = new GetApplicationClarificationsRequest(_applicationId, UserName);
            var result = await _orchestrator.GetClarificationViewModel(request);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetClarificationViewModel_returns_model()
        {
            var applyData = new ApplyData
            {
                ApplyDetails = new ApplyDetails
                {
                    UKPRN = Ukprn,
                    OrganisationName = OrganisationName
                }
            };
        
            var returnedRoatpApplicationResponse = new Apply
            {
                ApplicationId = _applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };
        
            var contactDetails = new ContactDetails {Email = Email};
            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(returnedRoatpApplicationResponse);
            _applyApiClient.Setup(x => x.GetContactDetails(_applicationId)).ReturnsAsync(contactDetails);
            
            const string status = SectionReviewStatus.Clarification;
            const string comment = "comments go here";

            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>
            {
                new GatewayPageAnswerSummary
                {
                    ApplicationId = _applicationId,
                    PageId = GatewayPageIds.OrganisationRisk,
                    Status = status,
                    Comments = comment
                }
            };
            
             _applyApiClient.Setup(x => x.GetGatewayPageAnswers(_applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var request = new GetApplicationClarificationsRequest(_applicationId, UserName);
        
            var viewModel = await _orchestrator.GetClarificationViewModel(request);
            
        
            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(Email, viewModel.ApplicationEmailAddress);
            Assert.AreEqual(Ukprn, viewModel.Ukprn);
            Assert.AreEqual(OrganisationName, viewModel.OrganisationName);
            Assert.AreEqual(1,viewModel.Sequences.Count);
            Assert.AreEqual("Organisation checks", viewModel.Sequences[0].SequenceTitle);
            Assert.AreEqual(1,viewModel.Sequences[0].Sections.Count);
            Assert.AreEqual("Organisation high risk", viewModel.Sequences[0].Sections[0].PageTitle);
            Assert.AreEqual(comment, viewModel.Sequences[0].Sections[0].Comment);
        }
    }
}
