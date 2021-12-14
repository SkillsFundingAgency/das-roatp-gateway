using System;
using System.Collections.Generic;
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
    public class OrganisationStatusTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;
        private Mock<IRoatpApiClient> _charityApiClient;

        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string ProviderName => "Mark's other workshop";
        private static string CompanyStatus => "active";
        private static string CompanyStatusWithCapitalisation => "Active";

        private static string CharityStatus => "closed";
        private static string CharityStatusWithCapitalisation => "Closed";
        private static string ProviderStatus = "registered";
        private static string ProviderStatusWithCapitalisation = "Registered";
        private static string UserName = "GatewayUser";
        private static string UserId = "GatewayUser@test.com";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _charityApiClient = new Mock<IRoatpApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _charityApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_company_and_charity_details()
        {
            var applicationId = Guid.NewGuid();
            var charityNumber = "44443333";
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
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = true,
                VerifiedbyCharityCommission = true,
                VerificationDetails = new List<VerificationDetails>
                {
                    new VerificationDetails {VerificationAuthority="Charity Commission", VerificationId=charityNumber}
                }
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityDetails()
            {
                Status = CharityStatus
            };
            _charityApiClient.Setup(x => x.GetCharityDetails(charityNumber)).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _charityApiClient.Verify(x => x.GetCharityDetails(charityNumber), Times.Once);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_company_details_only()
        {
            var applicationId = Guid.NewGuid();
            var charityNumber = "44443333";
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
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = true,
                VerifiedbyCharityCommission = false,
                VerificationDetails = new List<VerificationDetails>
                {
                    new VerificationDetails {VerificationAuthority="Charity Commission", VerificationId=charityNumber}
                }
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityDetails
            {
                Status = null
            };

            _charityApiClient.Setup(x => x.GetCharityDetails(charityNumber)).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.IsNull(viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _charityApiClient.Verify(x => x.GetCharityDetails(charityNumber), Times.Never);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_charity_details_only()
        {
            var applicationId = Guid.NewGuid();
            var charityNumber = "44443333";
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
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = false,
                VerifiedbyCharityCommission = true,
                VerificationDetails = new List<VerificationDetails>
                {
                    new VerificationDetails {VerificationAuthority="Charity Commission", VerificationId=charityNumber}
                }
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = null
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityDetails
            {
                Status = CharityStatus
            };

            _charityApiClient.Setup(x => x.GetCharityDetails(charityNumber)).ReturnsAsync(charityDetails);


            var request = new GetOrganisationStatusRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.IsNull(viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Never);
            _charityApiClient.Verify(x => x.GetCharityDetails(charityNumber), Times.Once);
        }
    }
}
