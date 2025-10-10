using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class InitialTeacherTrainingTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.InitialTeacherTraining;

        [Test]
        public void check_initial_teacher_training_details_are_returned()
        {
            var initialTeacherTraining = new InitialTeacherTraining { DoesOrganisationOfferInitialTeacherTraining = true, IsPostGradOnlyApprenticeship = false };
            ExperienceAndAccreditationApiClient.Setup(x => x.GetInitialTeacherTraining(ApplicationId)).ReturnsAsync(initialTeacherTraining);

            var request = new GetInitialTeacherTrainingRequest(ApplicationId, UserId, UserName);
            var response = Orchestrator.GetInitialTeacherTrainingViewModel(request);

            var viewModel = response.Result;

            Assert.That(viewModel.PageId, Is.EqualTo(GatewayPageIds.InitialTeacherTraining));
            AssertCommonDetails(viewModel);
            Assert.That(viewModel.DoesOrganisationOfferInitialTeacherTraining, Is.EqualTo(initialTeacherTraining.DoesOrganisationOfferInitialTeacherTraining));
            Assert.That(viewModel.IsPostGradOnlyApprenticeship, Is.EqualTo(initialTeacherTraining.IsPostGradOnlyApprenticeship));
        }
    }
}
