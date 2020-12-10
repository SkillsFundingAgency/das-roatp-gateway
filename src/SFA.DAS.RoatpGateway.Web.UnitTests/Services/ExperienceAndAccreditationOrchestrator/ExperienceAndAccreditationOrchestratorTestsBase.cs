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

        protected static string Ukprn => "12344321";
        protected static string UKRLPLegalName => "Mark's workshop";
        protected static string UserName = "GatewayUser";
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
            ApplyApiClient.Setup(x => x.GetPageCommonDetails(ApplicationId, GatewayPageId, UserName)).ReturnsAsync(CommonDetails);
        }

        protected void AssertCommonDetails(RoatpGatewayPageViewModel viewModel)
        {
            Assert.AreEqual(CommonDetails.ApplicationId, viewModel.ApplicationId);
            Assert.AreEqual(CommonDetails.PageId, viewModel.PageId);
            Assert.AreEqual(CommonDetails.ApplicationSubmittedOn, viewModel.ApplicationSubmittedOn);
            Assert.AreEqual(CommonDetails.SourcesCheckedOn, viewModel.SourcesCheckedOn);
            Assert.AreEqual(CommonDetails.LegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(CommonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(CommonDetails.GatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(CommonDetails.Status, viewModel.Status);
            Assert.AreEqual(CommonDetails.Comments, viewModel.Comments);
            Assert.AreEqual(CommonDetails.OutcomeMadeBy, viewModel.OutcomeMadeBy);
            Assert.AreEqual(CommonDetails.OutcomeMadeOn, viewModel.OutcomeMadeOn);

            // Note: If you change from a 'Pass' in Setup() then you'll have to amend this
            Assert.AreEqual(CommonDetails.Comments, viewModel.OptionPassText);
            Assert.Null(viewModel.OptionFailText);
            Assert.Null(viewModel.OptionInProgressText);
            Assert.Null(viewModel.OptionClarificationText);
        }
    }
}
