using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Refit;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.RegisterChecks.Orchestrator
{
    [TestFixture]
    public class RoatpTests
    {
        private GatewayRegisterChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpRegisterApiClient> _roatpApiClient;
        private Mock<ILogger<GatewayRegisterChecksOrchestrator>> _logger;

        private const string ukprn = "12345678";
        private const string UKRLPLegalName = "John's workshop";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";

        private const int roatpProviderTypeId = 1;
        private const int roatpStatusId = 1;

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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, It.IsAny<string>(), UserId, UserName)).ReturnsAsync(commonDetails);

            _applyApiClient.Setup(x => x.GetProviderRouteName(_applicationId)).ReturnsAsync($"{roatpProviderTypeId}");
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_not_on_register_details()
        {
            _roatpApiClient
                .Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString()))
                .ReturnsAsync(new ApiResponse<OrganisationResponse>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null));

            var request = new GetRoatpRequest(_applicationId, UserId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsFalse(viewModel.RoatpUkprnOnRegister);
            Assert.IsNull(viewModel.RoatpStatus);
            Assert.IsNull(viewModel.RoatpStatusDate);
            Assert.IsNull(viewModel.RoatpProviderRoute);
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_on_register_details()
        {
            var organisationResponse = new OrganisationResponse
            {
                Status = OrganisationStatus.Active,
                ProviderType = ProviderType.Main,
                StatusDate = DateTime.Now.AddMonths(-1)
            };
            var apiResponse = new ApiResponse<OrganisationResponse>(new HttpResponseMessage(HttpStatusCode.OK), organisationResponse, null);
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(apiResponse);

            var request = new GetRoatpRequest(_applicationId, UserId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsTrue(viewModel.RoatpUkprnOnRegister);
            Assert.AreEqual("Main provider", viewModel.RoatpProviderRoute);
            Assert.AreEqual("Active", viewModel.RoatpStatus);
            Assert.AreEqual(organisationResponse.StatusDate, viewModel.RoatpStatusDate);
            Assert.IsTrue(viewModel.RoatpUkprnOnRegister);
        }

        [TestCase("Main")]
        [TestCase("Supporting")]
        [TestCase("Employer")]
        public async Task check_orchestrator_builds_with_ProviderRouteName_details(string providerRouteName)
        {
            var organisationResponse = new OrganisationResponse
            {
                Status = OrganisationStatus.Active,
                ProviderType = ProviderType.Main,
                StatusDate = DateTime.Now.AddMonths(-1)
            };
            var apiResponse = new ApiResponse<OrganisationResponse>(new HttpResponseMessage(HttpStatusCode.OK), organisationResponse, null);
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(apiResponse);

            _applyApiClient.Setup(x => x.GetProviderRouteName(_applicationId)).ReturnsAsync(providerRouteName);

            var request = new GetRoatpRequest(_applicationId, UserId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.AreEqual(providerRouteName, viewModel.ApplyProviderRoute);
        }
    }
}
