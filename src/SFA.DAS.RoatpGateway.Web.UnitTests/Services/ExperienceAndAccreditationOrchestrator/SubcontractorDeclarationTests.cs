using System.IO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.UnitTests.Services.ExperienceAndAccreditationOrchestrator;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class SubcontractorDeclarationTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.SubcontractorDeclaration;

        [Test]
        public void check_subcontractor_declaration_details_are_returned()
        {
            var subcontractorDeclaration = new SubcontractorDeclaration { HasDeliveredTrainingAsSubcontractor = true, ContractFileName = "fileName" };
            ExperienceAndAccreditationApiClient.Setup(x => x.GetSubcontractorDeclaration(ApplicationId)).ReturnsAsync(subcontractorDeclaration);

            var request = new GetSubcontractorDeclarationRequest(ApplicationId, UserId, UserName);
            var response = Orchestrator.GetSubcontractorDeclarationViewModel(request);

            var viewModel = response.Result;

            Assert.That(viewModel.PageId, Is.EqualTo(GatewayPageIds.SubcontractorDeclaration));
            AssertCommonDetails(viewModel);
            Assert.That(viewModel.HasDeliveredTrainingAsSubcontractor, Is.EqualTo(subcontractorDeclaration.HasDeliveredTrainingAsSubcontractor));
            Assert.That(viewModel.ContractFileName, Is.EqualTo(subcontractorDeclaration.ContractFileName));
        }

        [Test]
        public void check_subcontractor_contract_file_is_returned()
        {
            var fileStreamResult = new FileStreamResult(new MemoryStream(), "application/pdf");
            ExperienceAndAccreditationApiClient.Setup(x => x.GetSubcontractorDeclarationContractFile(ApplicationId)).ReturnsAsync(fileStreamResult);

            var request = new GetSubcontractorDeclarationContractFileRequest(ApplicationId);
            var response = Orchestrator.GetSubcontractorDeclarationContractFile(request);

            var result = response.Result;

            Assert.That(result, Is.SameAs(fileStreamResult));
        }
    }
}
