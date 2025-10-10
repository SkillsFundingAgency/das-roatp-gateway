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
            Assert.That(viewModel.ApplicationId, Is.EqualTo(CommonDetails.ApplicationId));
            Assert.That(viewModel.PageId, Is.EqualTo(CommonDetails.PageId));
            Assert.That(viewModel.ApplicationSubmittedOn, Is.EqualTo(CommonDetails.ApplicationSubmittedOn));
            Assert.That(viewModel.SourcesCheckedOn, Is.EqualTo(CommonDetails.SourcesCheckedOn));
            Assert.That(viewModel.ApplyLegalName, Is.EqualTo(CommonDetails.LegalName));
            Assert.That(viewModel.Ukprn, Is.EqualTo(CommonDetails.Ukprn));
            Assert.That(viewModel.GatewayReviewStatus, Is.EqualTo(CommonDetails.GatewayReviewStatus));
            Assert.That(viewModel.Status, Is.EqualTo(CommonDetails.Status));
            Assert.That(viewModel.Comments, Is.EqualTo(CommonDetails.Comments));
            Assert.That(viewModel.OutcomeMadeBy, Is.EqualTo(CommonDetails.OutcomeMadeBy));
            Assert.That(viewModel.OutcomeMadeOn, Is.EqualTo(CommonDetails.OutcomeMadeOn));

            // Note: If you change from a 'Pass' in Setup() then you'll have to amend this
            Assert.That(viewModel.OptionPassText, Is.EqualTo(CommonDetails.Comments));
            Assert.That(viewModel.OptionFailText, Is.Null);
            Assert.That(viewModel.OptionInProgressText, Is.Null);
            Assert.That(viewModel.OptionClarificationText, Is.Null);
        }
    }
}
