using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.RegisterChecks.Orchestrator
{
    [TestFixture]
    public class RoatpTests
    {
        private GatewayRegisterChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpRegisterApiClient> _roatpApiClient;
        private Mock<ILogger<GatewayRegisterChecksOrchestrator>> _logger;

        private static string ukprn = "12345678";
        private static string UKRLPLegalName = "John's workshop";
        private static string UserName = "GatewayUser";

        private static int roatpProviderTypeId = 1;
        private static int roatpStatusId = 1;

        private readonly Guid _applicationId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _roatpApiClient = new Mock<IRoatpRegisterApiClient>();
            _logger = new Mock<ILogger<GatewayRegisterChecksOrchestrator>>();
            _orchestrator = new GatewayRegisterChecksOrchestrator(_applyApiClient.Object, _roatpApiClient.Object, _logger.Object);

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationId = _applicationId,
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, It.IsAny<string>(), UserName)).ReturnsAsync(commonDetails);

            _applyApiClient.Setup(x => x.GetProviderRouteName(_applicationId)).ReturnsAsync($"{roatpProviderTypeId}");

            var providerType = new ProviderType
            {
                Id = roatpProviderTypeId,
                Type = $"{roatpProviderTypeId}"
            };
            _roatpApiClient.Setup(x => x.GetProviderTypes()).ReturnsAsync(new List<ProviderType> { providerType });

            var organisationStatus = new OrganisationStatus
            {
                Id = roatpStatusId,
                Status = $"{roatpStatusId}"
            };
            _roatpApiClient.Setup(x => x.GetOrganisationStatuses(It.IsAny<int?>())).ReturnsAsync(new List<OrganisationStatus> { organisationStatus });
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_not_on_register_details()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(organisationRegisterStatus);

            var request = new GetRoatpRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsFalse(viewModel.RoatpUkprnOnRegister);
            Assert.IsNull(viewModel.RoatpStatus);
            Assert.IsNull(viewModel.RoatpStatusDate);
            Assert.IsNull(viewModel.RoatpProviderRoute);
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_on_register_details()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = true,
                StatusId = roatpStatusId,
                ProviderTypeId = roatpProviderTypeId,
                StatusDate = DateTime.Now.AddMonths(-1)
            };
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(organisationRegisterStatus);

            var request = new GetRoatpRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsTrue(viewModel.RoatpUkprnOnRegister);
            Assert.IsNotNull(viewModel.RoatpStatus);
            Assert.IsNotNull(viewModel.RoatpStatusDate);
            Assert.IsNotNull(viewModel.RoatpProviderRoute);
        }

        [TestCase("Main")]
        [TestCase("Supporting")]
        [TestCase("Employer")]
        public async Task check_orchestrator_builds_with_ProviderRouteName_details(string providerRouteName)
        {
            _applyApiClient.Setup(x => x.GetProviderRouteName(_applicationId)).ReturnsAsync(providerRouteName);

            var request = new GetRoatpRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.AreEqual(providerRouteName, viewModel.ApplyProviderRoute);
        }
    }
}
