using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.RegisterChecks.Orchestrator
{
    [TestFixture]
    public class RoepaoTests
    {
        private GatewayRegisterChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpRegisterApiClient> _roatpApiClient;
        private Mock<ILogger<GatewayRegisterChecksOrchestrator>> _logger;

        private static string ukprn = "12345678";
        private static string UKRLPLegalName = "John's workshop";
        private static string UserName = "GatewayUser";

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
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, It.IsAny<string>(), UserName)).ReturnsAsync(commonDetails);
        }

        [Test]
        public async Task check_orchestrator_builds_with_roepao_details()
        {
            var request = new GetRoepaoRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoepaoViewModel(request);

            // NOTE: We only grab Common Details at the moment
            Assert.IsNotNull(viewModel.Ukprn);
            Assert.IsNotNull(viewModel.ApplyLegalName);
        }
    }
}
