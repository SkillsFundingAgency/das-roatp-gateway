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

            Assert.That(viewModel.PageId, Is.EqualTo(GatewayPageIds.Ofsted));
            AssertCommonDetails(viewModel);

            Assert.That(viewModel.FullInspectionApprenticeshipGrade, Is.EqualTo(ofstedDetails.FullInspectionApprenticeshipGrade));
            Assert.That(viewModel.FullInspectionOverallEffectivenessGrade, Is.EqualTo(ofstedDetails.FullInspectionOverallEffectivenessGrade));
            Assert.That(viewModel.GradeWithinTheLast3Years, Is.EqualTo(ofstedDetails.GradeWithinTheLast3Years));
            Assert.That(viewModel.HasHadFullInspection, Is.EqualTo(ofstedDetails.HasHadFullInspection));
            Assert.That(viewModel.HasHadMonitoringVisit, Is.EqualTo(ofstedDetails.HasHadMonitoringVisit));
            Assert.That(viewModel.HasHadShortInspectionWithinLast3Years, Is.EqualTo(ofstedDetails.HasHadShortInspectionWithinLast3Years));
            Assert.That(viewModel.HasMaintainedFullGradeInShortInspection, Is.EqualTo(ofstedDetails.HasMaintainedFullGradeInShortInspection));
            Assert.That(viewModel.HasMaintainedFundingSinceInspection, Is.EqualTo(ofstedDetails.HasMaintainedFundingSinceInspection));
            Assert.That(viewModel.ReceivedFullInspectionGradeForApprenticeships, Is.EqualTo(ofstedDetails.ReceivedFullInspectionGradeForApprenticeships));
            Assert.That(viewModel.Has2MonitoringVisitsGradedInadequate, Is.EqualTo(ofstedDetails.Has2MonitoringVisitsGradedInadequate)); 
            Assert.That(viewModel.HasMonitoringVisitGradedInadequateInLast18Months, Is.EqualTo(ofstedDetails.HasMonitoringVisitGradedInadequateInLast18Months));
        }
    }
}
