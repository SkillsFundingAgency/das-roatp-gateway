using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.RegisterChecks.Orchestrator
{
    [TestFixture]
    public class RoepaoTests
    {
        private GatewayRegisterChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpRegisterApiClient> _roatpApiClient;
        private Mock<ILogger<GatewayRegisterChecksOrchestrator>> _logger;

        private const string ukprn = "12345678";
        private const string UKRLPLegalName = "John's workshop";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";

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
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, It.IsAny<string>(), UserId, UserName)).ReturnsAsync(commonDetails);
        }

        [Test]
        public async Task check_orchestrator_builds_with_roepao_details()
        {
            var request = new GetRoepaoRequest(_applicationId, UserId, UserName);

            var viewModel = await _orchestrator.GetRoepaoViewModel(request);

            // NOTE: We only grab Common Details at the moment
            Assert.That(viewModel.Ukprn, Is.Not.Null);
            Assert.That(viewModel.ApplyLegalName, Is.Not.Null);
        }
    }
}
