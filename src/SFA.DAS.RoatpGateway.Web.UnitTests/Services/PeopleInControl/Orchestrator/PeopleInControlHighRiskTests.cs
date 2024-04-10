using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.PeopleInControl.Orchestrator
{
    [TestFixture]
    public class PeopleInControlHighRiskTests
    {
        private PeopleInControlOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private Mock<ILogger<PeopleInControlOrchestrator>> _logger;

        private const string ukprn = "12344321";
        private const string UKRLPLegalName = "Mark's workshop";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";
        private string GatewayPageId => GatewayPageIds.PeopleInControlRisk;

        const string PersonInControlName = "Bob";
        const string PersonInControlDob = "Jan 1990";
        const string DirectorsPostfix = "-directors";
        const string PscPostfix = "-psc";
        const string TrusteesPostfix = "-trustee";
        const string WhosInControlPostfix = "-wic";

        readonly Guid _applicationId = Guid.NewGuid();
        private GatewayCommonDetails _commonDetails;

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _logger = new Mock<ILogger<PeopleInControlOrchestrator>>();
            _orchestrator = new PeopleInControlOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _logger.Object);

            _commonDetails = new GatewayCommonDetails
            {
                ApplicationId = _applicationId,
                PageId = GatewayPageId,
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                SourcesCheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn,
                GatewayReviewStatus = "RevStatus",
                Comments = "Fail",
                Status = "Fail"
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, GatewayPageId, UserId, UserName)).ReturnsAsync(_commonDetails);
        }

        [Test]
        public void check_people_in_control_high_risk_common_details_are_returned()
        {

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.That(GatewayPageId, Is.EqualTo(viewModel.PageId));
            Assert.That(_applicationId, Is.EqualTo(viewModel.ApplicationId));
            Assert.That(_commonDetails.Comments, Is.EqualTo(viewModel.OptionFailText));
            Assert.That(viewModel.OptionInProgressText, Is.Null);
            Assert.That(viewModel.OptionClarificationText, Is.Null);
            Assert.That(viewModel.OptionPassText, Is.Null);
            Assert.That(_commonDetails.Status, Is.EqualTo(viewModel.Status));
            Assert.That(_commonDetails.Ukprn, Is.EqualTo(viewModel.Ukprn));
            Assert.That(_commonDetails.LegalName, Is.EqualTo(viewModel.ApplyLegalName));
        }


        [Test]
        public void check_people_in_control_high_risk_director_details_from_cached_data_are_returned()
        {

            var directorsFromSubmitted = new List<PersonInControl>{
                new PersonInControl{
                    Name =PersonInControlName + DirectorsPostfix,
                    MonthYearOfBirth = PersonInControlDob + DirectorsPostfix ,
                } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromCompaniesHouse(_applicationId)).ReturnsAsync(directorsFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.CompanyDirectorsData.PeopleInControl.Count));
            Assert.That(viewModel.CompanyDirectorsData.PeopleInControl.First().Name, Is.EqualTo(directorsFromSubmitted.First().Name));
            Assert.That(viewModel.CompanyDirectorsData.PeopleInControl.First().MonthYearOfBirth, Is.EqualTo(directorsFromSubmitted.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_high_risk_psc_details_from_submitted_are_returned()
        {

            var pcsFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix,
                    MonthYearOfBirth = PersonInControlDob + PscPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromCompaniesHouse(_applicationId)).ReturnsAsync(pcsFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.PscData.PeopleInControl.Count));
            Assert.That(viewModel.PscData.PeopleInControl.First().Name, Is.EqualTo(pcsFromSubmitted.First().Name));
            Assert.That(viewModel.PscData.PeopleInControl.First().MonthYearOfBirth, Is.EqualTo(pcsFromSubmitted.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_trustee_details_from_submitted_are_returned()
        {
            var trusteesFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix,
                    MonthYearOfBirth = PersonInControlDob + TrusteesPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromCharityCommission(_applicationId)).ReturnsAsync(trusteesFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;
            Assert.That(1, Is.EqualTo(viewModel.TrusteeData.PeopleInControl.Count));

            Assert.That(viewModel.TrusteeData.PeopleInControl.First().Name, Is.EqualTo(trusteesFromSubmitted.First().Name));
            Assert.That(viewModel.TrusteeData.PeopleInControl.First().MonthYearOfBirth, Is.EqualTo(trusteesFromSubmitted.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_whos_in_control_details_from_submitted_are_returned()
        {
            var whosInControlFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ WhosInControlPostfix,
                    MonthYearOfBirth = PersonInControlDob + WhosInControlPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetWhosInControlFromSubmitted(_applicationId)).ReturnsAsync(whosInControlFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.WhosInControlData.PeopleInControl.Count));
            Assert.That(viewModel.WhosInControlData.PeopleInControl.First().Name, Is.EqualTo(whosInControlFromSubmitted.First().Name));
            Assert.That(viewModel.WhosInControlData.PeopleInControl.First().MonthYearOfBirth, Is.EqualTo(whosInControlFromSubmitted.First().MonthYearOfBirth));
        }
    }
}
