using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class WebsiteAddressTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private const string ukprn = "12345678";
        private const string UKRLPLegalName = "John's workshop";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), Mock.Of<IRoatpApiClient>(), _logger.Object);
        }

        [TestCase("http://www.OrganisationWebSite.co.uk", "http://www.UkrlpApiWebsite.co.uk")]
        [TestCase(null, "http://www.UkrlpApiWebsite.co.uk")]
        [TestCase("http://www.OrganisationWebSite.co.uk", null)]
        [TestCase(null, null)]
        public void check_orchestrator_builds_with_website_address(string organisationWebsite, string ukrlpApiWebsite)
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            _applyApiClient.Setup(x => x.GetOrganisationWebsiteAddress(applicationId)).ReturnsAsync(organisationWebsite);

            var ukrlpDetails = new ProviderDetails
            {
                ContactDetails = new List<ProviderContact>()
                {
                    new ProviderContact
                    {
                        ContactWebsiteAddress = ukrlpApiWebsite,
                        ContactType = RoatpGatewayConstants.ProviderContactDetailsTypeLegalIdentifier
                    }
                }
            };

            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var request = new GetWebsiteRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetWebsiteViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(organisationWebsite, viewModel.SubmittedWebsite);
            Assert.AreEqual(ukrlpApiWebsite, viewModel.UkrlpWebsite);
        }
    }
}
