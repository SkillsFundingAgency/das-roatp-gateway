using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class IcoNumberTests
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
        public void check_orchestrator_builds_with_ico_number()
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
            var expectedOrganisationAddress = "1 QnA Street, Second Address, Third Address, Forth Address, Coventry, CV2 1WT";

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


            // SubmittedApplicationIcoNumber
            var expectedIcoNumberIcoNumber = "1234QWER";
            var icoNumberResult = new IcoNumber { Value = "1234QWER" };


            _applyApiClient.Setup(x => x.GetIcoNumber(It.IsAny<Guid>())).ReturnsAsync(icoNumberResult);



            var request = new GetIcoNumberRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetIcoNumberViewModel(request);

            var viewModel = response.Result;

            Assert.That(UKRLPLegalName, Is.EqualTo(viewModel.ApplyLegalName));
            Assert.That(ukprn, Is.EqualTo(viewModel.Ukprn));
            Assert.That(expectedOrganisationAddress, Is.EqualTo(viewModel.OrganisationAddress));
            Assert.That(expectedIcoNumberIcoNumber, Is.EqualTo(viewModel.IcoNumber));
        }
    }
}
