﻿using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class OneInTwelveMonthsTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string Ukprn = "12345678";
        private static string UKRLPLegalName = "John's workshop";
        private static string ProviderRouteName = "Main";
        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _logger.Object);
        }

        [TestCase("YES")]
        [TestCase("NO")]
        public async Task check_orchestrator_builds_with_one_in_twelve_months(string oneInTwelveMonths)
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                ProviderRouteName = ProviderRouteName,
                Ukprn = Ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            _applyApiClient.Setup(x => x.GetOneInTwelveMonths(applicationId)).ReturnsAsync(oneInTwelveMonths);

            var request = new GetOneInTwelveMonthsRequest(applicationId, UserName);

            var viewModel = await _orchestrator.GetOneInTwelveMonthsViewModel(request);

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderRouteName, viewModel.ApplicationRoute);
            Assert.AreEqual(Ukprn, viewModel.Ukprn);
            Assert.AreEqual(commonDetails.ApplicationSubmittedOn, viewModel.ApplicationSubmittedOn);
            Assert.AreEqual(oneInTwelveMonths, viewModel.SubmittedOneInTwelveMonths);
        }
    }
}
