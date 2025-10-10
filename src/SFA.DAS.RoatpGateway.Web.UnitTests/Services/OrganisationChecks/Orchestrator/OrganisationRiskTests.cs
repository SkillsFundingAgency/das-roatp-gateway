using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class OrganisationRiskTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private const string ukprn = "12345678";
        private const string UKRLPLegalName = "John LTD.";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";
        private const string CompanyNumber = "12345678";
        private const string CharityNumber = "87654321";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _logger.Object);
        }

        [TestCase("Company and charity", "John Training and Consultancy")]
        [TestCase(null, "John Training and Consultancy")]
        [TestCase("Company and charity", null)]
        [TestCase(null, null)]
        public void Check_organisation_risk_details_are_returned(string organisationType, string tradingName)
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now, 
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, pageId, UserId, UserName)).ReturnsAsync(commonDetails);

            _organisationSummaryApiClient.Setup(x => x.GetTypeOfOrganisation(applicationId)).ReturnsAsync(organisationType);
            _organisationSummaryApiClient.Setup(x => x.GetCharityNumber(applicationId)).ReturnsAsync(CharityNumber);
            _organisationSummaryApiClient.Setup(x => x.GetCompanyNumber(applicationId)).ReturnsAsync(CompanyNumber);
            _applyApiClient.Setup(x => x.GetTradingName(applicationId)).ReturnsAsync(tradingName);
            
            var request = new GetOrganisationRiskRequest(applicationId, UserId, UserName);

            var response = _orchestrator.GetOrganisationRiskViewModel(request);

            var viewModel = response.Result;

            Assert.That(viewModel.ApplyLegalName, Is.EqualTo(UKRLPLegalName));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
            Assert.That(viewModel.OrganisationType, Is.EqualTo(organisationType));
            Assert.That(viewModel.TradingName, Is.EqualTo(tradingName));
            Assert.That(viewModel.CompanyNumber, Is.EqualTo(CompanyNumber));
            Assert.That(viewModel.CharityNumber, Is.EqualTo(CharityNumber));

        }
    }
}
