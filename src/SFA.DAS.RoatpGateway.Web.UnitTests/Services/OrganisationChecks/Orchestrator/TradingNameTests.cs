using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class TradingNameTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string PageId => "1-20";
        private static string ukprn => "12344321";
        private static string UKRLPTradingName => "Mark's workshop";
        private static string LegalName => "My Legal Name";

        private static string UserName = "GatewayUser";
        private static string UserId = "GatewayUser@test.com";
        private static string ApplyTradingName = "My Trading Name";

        private static string QuestionId => "PRE-46";
        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), Mock.Of<IRoatpApiClient>(),_logger.Object);
        }

        [Test]
        public void check_trading_name_orchestrator_builds_with_expected_details()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = LegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderAliases = new List<ProviderAlias>()
                {
                    new ProviderAlias{Alias = UKRLPTradingName}

                }
            };

            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);
            _applyApiClient.Setup(x => x.GetTradingName(It.IsAny<Guid>())).ReturnsAsync(ApplyTradingName);

            var request = new GetTradingNameRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetTradingNameViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPTradingName, viewModel.UkrlpTradingName);
            Assert.AreEqual(ApplyTradingName, viewModel.ApplyTradingName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }


    }
}
