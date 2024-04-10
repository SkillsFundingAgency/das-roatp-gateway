using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.ExperienceAndAccreditationOrchestrator
{
    public abstract class ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected GatewayExperienceAndAccreditationOrchestrator Orchestrator;
        protected Mock<IRoatpApplicationApiClient> ApplyApiClient;
        protected Mock<IRoatpExperienceAndAccreditationApiClient> ExperienceAndAccreditationApiClient;
        protected Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>> Logger;
        protected abstract string GatewayPageId { get; }

        protected const string Ukprn = "12344321";
        protected const string UKRLPLegalName = "Mark's workshop";
        protected const string UserName = "GatewayUser";
        protected const string UserId = "GatewayUser@test.com";
        protected GatewayCommonDetails CommonDetails;
        protected Guid ApplicationId;

        [SetUp]
        public void Setup()
        {
            ApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            ExperienceAndAccreditationApiClient = new Mock<IRoatpExperienceAndAccreditationApiClient>();
            Logger = new Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>>();
            Orchestrator = new GatewayExperienceAndAccreditationOrchestrator(ApplyApiClient.Object, ExperienceAndAccreditationApiClient.Object, Logger.Object);

            ApplicationId = Guid.NewGuid();

            CommonDetails = new GatewayCommonDetails
            {
                ApplicationId = ApplicationId,
                PageId = GatewayPageId,
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now.AddHours(-2),
                LegalName = UKRLPLegalName,
                Ukprn = Ukprn,
                GatewayReviewStatus = "RevStatus",
                Status = "Pass",
                Comments = "No comment",
                OutcomeMadeBy = UserName,
                OutcomeMadeOn = DateTime.Now
            };
            ApplyApiClient.Setup(x => x.GetPageCommonDetails(ApplicationId, GatewayPageId, UserId, UserName)).ReturnsAsync(CommonDetails);
        }

        protected void AssertCommonDetails(RoatpGatewayPageViewModel viewModel)
        {
            Assert.That(CommonDetails.ApplicationId, Is.EqualTo(viewModel.ApplicationId));
            Assert.That(CommonDetails.PageId, Is.EqualTo(viewModel.PageId));
            Assert.That(CommonDetails.ApplicationSubmittedOn, Is.EqualTo(viewModel.ApplicationSubmittedOn));
            Assert.That(CommonDetails.SourcesCheckedOn, Is.EqualTo(viewModel.SourcesCheckedOn));
            Assert.That(CommonDetails.LegalName, Is.EqualTo(viewModel.ApplyLegalName));
            Assert.That(CommonDetails.Ukprn, Is.EqualTo(viewModel.Ukprn));
            Assert.That(CommonDetails.GatewayReviewStatus, Is.EqualTo(viewModel.GatewayReviewStatus));
            Assert.That(CommonDetails.Status, Is.EqualTo(viewModel.Status));
            Assert.That(CommonDetails.Comments, Is.EqualTo(viewModel.Comments));
            Assert.That(CommonDetails.OutcomeMadeBy, Is.EqualTo(viewModel.OutcomeMadeBy));
            Assert.That(CommonDetails.OutcomeMadeOn, Is.EqualTo(viewModel.OutcomeMadeOn));

            // Note: If you change from a 'Pass' in Setup() then you'll have to amend this
            Assert.That(CommonDetails.Comments, Is.EqualTo(viewModel.OptionPassText));
            Assert.That(viewModel.OptionFailText, Is.Null);
            Assert.That(viewModel.OptionInProgressText, Is.Null);
            Assert.That(viewModel.OptionClarificationText, Is.Null);
        }
    }
}
