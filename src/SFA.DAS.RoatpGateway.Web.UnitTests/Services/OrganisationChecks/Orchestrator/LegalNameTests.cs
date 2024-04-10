using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Domain.CompaniesHouse;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class LegalNameTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string ProviderName => "Mark's other workshop";
        private static string CompanyName => "Companies House Name";

        private static string CharityName => "Charity commission Name";

        private static string UserName = "GatewayUser";

        private static string UserId = "GatewayUser@test.com";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _logger.Object);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_company_and_charity_details()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                VerifiedbyCharityCommission = true,
                VerifiedByCompaniesHouse = true
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                CompanyName = CompanyName
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                CharityName = CharityName
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.That(UKRLPLegalName, Is.EqualTo(viewModel.ApplyLegalName));
            Assert.That(ProviderName, Is.EqualTo(viewModel.UkrlpLegalName));
            Assert.That(CompanyName, Is.EqualTo(viewModel.CompaniesHouseLegalName));
            Assert.That(CharityName, Is.EqualTo(viewModel.CharityCommissionLegalName));
            Assert.That(ukprn, Is.EqualTo(viewModel.Ukprn));
            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Once);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_company_details_only()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                VerifiedByCompaniesHouse = true,
                VerifiedbyCharityCommission = false
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                CompanyName = CompanyName
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            CharityCommissionSummary charityDetails = null;
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.That(UKRLPLegalName, Is.EqualTo(viewModel.ApplyLegalName));
            Assert.That(ProviderName, Is.EqualTo(viewModel.UkrlpLegalName));
            Assert.That(CompanyName, Is.EqualTo(viewModel.CompaniesHouseLegalName));
            Assert.That(viewModel.CharityCommissionLegalName, Is.Null);
            Assert.That(ukprn, Is.EqualTo(viewModel.Ukprn));
            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Never);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_charity_details_only()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                VerifiedByCompaniesHouse = false,
                VerifiedbyCharityCommission = true
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            CompaniesHouseSummary companiesHouseDetails = null;
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                CharityName = CharityName
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.That(UKRLPLegalName, Is.EqualTo(viewModel.ApplyLegalName));
            Assert.That(ProviderName, Is.EqualTo(viewModel.UkrlpLegalName));
            Assert.That(viewModel.CompaniesHouseLegalName, Is.Null);
            Assert.That(CharityName, Is.EqualTo(viewModel.CharityCommissionLegalName));
            Assert.That(ukprn, Is.EqualTo(viewModel.Ukprn));
            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Never);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Once);
        }

    }
}
