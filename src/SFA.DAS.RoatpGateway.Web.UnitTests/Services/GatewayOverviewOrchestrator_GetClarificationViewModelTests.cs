﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

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
            Assert.That(result, Is.Null);
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

            var contactDetails = new ContactDetails { Email = Email };
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


            Assert.That(_applicationId, Is.EqualTo(viewModel.ApplicationId));
            Assert.That(Email, Is.EqualTo(viewModel.ApplicationEmailAddress));
            Assert.That(Ukprn, Is.EqualTo(viewModel.Ukprn));
            Assert.That(OrganisationName, Is.EqualTo(viewModel.OrganisationName));
            Assert.That(1, Is.EqualTo(viewModel.Sequences.Count));
            Assert.That("Organisation checks", Is.EqualTo(viewModel.Sequences[0].SequenceTitle));
            Assert.That(1, Is.EqualTo(viewModel.Sequences[0].Sections.Count));
            Assert.That("Organisation high risk", Is.EqualTo(viewModel.Sequences[0].Sections[0].PageTitle));
            Assert.That(comment, Is.EqualTo(viewModel.Sequences[0].Sections[0].Comment));
        }
    }
}
