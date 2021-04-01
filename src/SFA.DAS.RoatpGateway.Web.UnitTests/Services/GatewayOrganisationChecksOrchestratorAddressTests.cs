using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewayOrganisationChecksOrchestratorAddressTests
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
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _logger.Object);
        }

        [Test]
        public void check_orchestrator_builds_with_both_addresses_details()
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

            // SubmittedApplicationAddress
            var SubmittedApplicationAddress = "1 QnA Street, Second Address, Third Address, Forth Address, Coventry, CV2 1WT";

            var organisationAddressResult = new ContactAddress
            {
                Address1 = "1 QnA Street",
                Address2 = "Second Address",
                Address3 = "Third Address",
                Address4 = "Forth Address",
                Town = "Coventry",
                PostCode = "CV2 1WT"
            };

            _applyApiClient.Setup(x => x.GetOrganisationAddress(It.IsAny<Guid>())).ReturnsAsync(organisationAddressResult);

            // UkrlpAddress
            var UkrlpAddress = "1 First Street, Second Address, Third Address, Forth Address, Coventry, CV1 2WT";

            var ukrlpDetails = new ProviderDetails
            {
                ContactDetails = new List<ProviderContact>
                                    { new  ProviderContact { ContactAddress = new ContactAddress {
                                                                                                    Address1 = "1 First Street",
                                                                                                    Address2 = "Second Address",
                                                                                                    Address3 = "Third Address",
                                                                                                    Address4 = "Forth Address",
                                                                                                    Town = "Coventry",
                                                                                                    PostCode = "CV1 2WT"
                                                                                                  }
                                                            }
                                    }
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var request = new GetAddressRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetAddressViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(SubmittedApplicationAddress, viewModel.SubmittedApplicationAddress);
            Assert.AreEqual(UkrlpAddress, viewModel.UkrlpAddress);
        }


    }
}
