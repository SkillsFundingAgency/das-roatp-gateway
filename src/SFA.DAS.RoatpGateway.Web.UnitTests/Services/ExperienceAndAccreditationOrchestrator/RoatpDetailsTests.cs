using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class OfstedDetailsTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.Ofsted;

        [Test]
        public void check_ofsted_details_are_returned()
        {
            var ofstedDetails = new OfstedDetails
            {
                FullInspectionApprenticeshipGrade = "Pass",
                FullInspectionOverallEffectivenessGrade = "Fail",
                GradeWithinTheLast3Years = true,
                HasHadFullInspection = false,
                HasHadMonitoringVisit = true,
                HasHadShortInspectionWithinLast3Years = false,
                HasMaintainedFullGradeInShortInspection = true,
                HasMaintainedFundingSinceInspection = false,
                ReceivedFullInspectionGradeForApprenticeships = true,
                Has2MonitoringVisitsGradedInadequate = false,
                HasMonitoringVisitGradedInadequateInLast18Months = true
            };
            ExperienceAndAccreditationApiClient.Setup(x => x.GetOfstedDetails(ApplicationId)).ReturnsAsync(ofstedDetails);

            var request = new GetOfstedDetailsRequest(ApplicationId, UserId, UserName);
            var response = Orchestrator.GetOfstedDetailsViewModel(request);

            var viewModel = response.Result;

            Assert.That(GatewayPageIds.Ofsted, Is.EqualTo(viewModel.PageId));
            AssertCommonDetails(viewModel);

            Assert.That(ofstedDetails.FullInspectionApprenticeshipGrade, Is.EqualTo(viewModel.FullInspectionApprenticeshipGrade));
            Assert.That(ofstedDetails.FullInspectionOverallEffectivenessGrade, Is.EqualTo(viewModel.FullInspectionOverallEffectivenessGrade));
            Assert.That(ofstedDetails.GradeWithinTheLast3Years, Is.EqualTo(viewModel.GradeWithinTheLast3Years));
            Assert.That(ofstedDetails.HasHadFullInspection, Is.EqualTo(viewModel.HasHadFullInspection));
            Assert.That(ofstedDetails.HasHadMonitoringVisit, Is.EqualTo(viewModel.HasHadMonitoringVisit));
            Assert.That(ofstedDetails.HasHadShortInspectionWithinLast3Years, Is.EqualTo(viewModel.HasHadShortInspectionWithinLast3Years));
            Assert.That(ofstedDetails.HasMaintainedFullGradeInShortInspection, Is.EqualTo(viewModel.HasMaintainedFullGradeInShortInspection));
            Assert.That(ofstedDetails.HasMaintainedFundingSinceInspection, Is.EqualTo(viewModel.HasMaintainedFundingSinceInspection));
            Assert.That(ofstedDetails.ReceivedFullInspectionGradeForApprenticeships, Is.EqualTo(viewModel.ReceivedFullInspectionGradeForApprenticeships));
            Assert.That(ofstedDetails.Has2MonitoringVisitsGradedInadequate, Is.EqualTo(viewModel.Has2MonitoringVisitsGradedInadequate));
            Assert.That(ofstedDetails.HasMonitoringVisitGradedInadequateInLast18Months, Is.EqualTo(viewModel.HasMonitoringVisitGradedInadequateInLast18Months));
        }
    }
}
