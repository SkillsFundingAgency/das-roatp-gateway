using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class OfficeForStudentsTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.OfficeForStudents;

        [TestCase("Yes", true)]
        [TestCase("No", false)]
        public void check_office_for_students_details_are_returned(string returnedAnswer, bool expectedResult)
        {
            ExperienceAndAccreditationApiClient.Setup(x => x.GetOfficeForStudents(ApplicationId)).ReturnsAsync(returnedAnswer);

            var request = new GetOfficeForStudentsRequest(ApplicationId, UserId, UserName);
            var response = Orchestrator.GetOfficeForStudentsViewModel(request);

            var viewModel = response.Result;

            Assert.That(GatewayPageIds.OfficeForStudents, Is.EqualTo(viewModel.PageId));
            AssertCommonDetails(viewModel);
            Assert.That(expectedResult, Is.EqualTo(viewModel.IsOrganisationFundedByOfficeForStudents));
        }
    }
}
