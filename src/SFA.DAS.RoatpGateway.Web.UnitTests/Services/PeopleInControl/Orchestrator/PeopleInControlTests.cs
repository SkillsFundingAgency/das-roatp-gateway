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
    public class PeopleInControlTests
    {
        private PeopleInControlOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private Mock<ILogger<PeopleInControlOrchestrator>> _logger;

        private const string ukprn = "12344321";
        private const string UKRLPLegalName = "Mark's workshop";
        private const string UserName = "GatewayUser";
        private const string UserId = "GatewayUser@test.com";
        private const string GatewayPageId = GatewayPageIds.PeopleInControl;

        const string PersonInControlName = "Bob";
        const string PersonInControlDob = "Jan 1990";
        const string SourceExternal = "-external";
        const string SourceSubmitted = "-submitted";
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
        public void check_people_in_control_common_details_are_returned()
        {

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

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
        public void check_people_in_control_director_details_from_companies_house_are_returned()
        {
            var directorsFromCompaniesHouse = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName + DirectorsPostfix + SourceExternal,
                    MonthYearOfBirth = PersonInControlDob + DirectorsPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromCompaniesHouse(_applicationId))
                .ReturnsAsync(directorsFromCompaniesHouse);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.CompanyDirectorsData.FromExternalSource.Count));

            Assert.That(viewModel.CompanyDirectorsData.FromExternalSource.First().Name,
                Is.EqualTo(directorsFromCompaniesHouse.First().Name));
            Assert.That(viewModel.CompanyDirectorsData.FromExternalSource.First().MonthYearOfBirth,
                Is.EqualTo(directorsFromCompaniesHouse.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_director_details_from_submitted_are_returned()
        {

            var directorsFromSubmitted = new List<PersonInControl>{
                new PersonInControl{
                    Name =PersonInControlName + DirectorsPostfix + SourceSubmitted,
                    MonthYearOfBirth = PersonInControlDob + DirectorsPostfix + SourceSubmitted
                } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromSubmitted(_applicationId)).ReturnsAsync(directorsFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.CompanyDirectorsData.FromSubmittedApplication.Count));
            Assert.That(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().Name, Is.EqualTo(directorsFromSubmitted.First().Name));
            Assert.That(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().MonthYearOfBirth, Is.EqualTo(directorsFromSubmitted.First().MonthYearOfBirth));
        }


        [Test]
        public void check_people_in_control_psc_details_from_companies_house_are_returned()
        {

            var pscsFromCompaniesHouse = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix + SourceExternal,
                    MonthYearOfBirth = PersonInControlDob + PscPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromCompaniesHouse(_applicationId)).ReturnsAsync(pscsFromCompaniesHouse);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.PscData.FromExternalSource.Count));

            Assert.That(viewModel.PscData.FromExternalSource.First().Name, Is.EqualTo(pscsFromCompaniesHouse.First().Name));
            Assert.That(viewModel.PscData.FromExternalSource.First().MonthYearOfBirth, Is.EqualTo(pscsFromCompaniesHouse.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_psc_details_from_submitted_are_returned()
        {

            var pcsFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix + SourceSubmitted,
                    MonthYearOfBirth = PersonInControlDob + PscPostfix + SourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromSubmitted(_applicationId)).ReturnsAsync(pcsFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.PscData.FromSubmittedApplication.Count));
            Assert.That(viewModel.PscData.FromSubmittedApplication.First().Name, Is.EqualTo(pcsFromSubmitted.First().Name));
            Assert.That(viewModel.PscData.FromSubmittedApplication.First().MonthYearOfBirth, Is.EqualTo(pcsFromSubmitted.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_trustee_details_from_charity_commission_are_returned()
        {
            var trusteesFromCharityCommission = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromCharityCommission(_applicationId)).ReturnsAsync(trusteesFromCharityCommission);


            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;
            Assert.That(1, Is.EqualTo(viewModel.TrusteeData.FromExternalSource.Count));
            Assert.That(viewModel.TrusteeData.FromExternalSource.First().Name, Is.EqualTo(trusteesFromCharityCommission.First().Name));
            Assert.That(viewModel.TrusteeData.FromExternalSource.First().MonthYearOfBirth, Is.EqualTo(trusteesFromCharityCommission.First().MonthYearOfBirth));

        }

        [Test]
        public void check_people_in_control_trustee_details_from_submitted_are_returned()
        {
            var trusteesFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix + SourceExternal,
                    MonthYearOfBirth = PersonInControlDob + TrusteesPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromSubmitted(_applicationId)).ReturnsAsync(trusteesFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;
            Assert.That(1, Is.EqualTo(viewModel.TrusteeData.FromSubmittedApplication.Count));

            Assert.That(viewModel.TrusteeData.FromSubmittedApplication.First().Name, Is.EqualTo(trusteesFromSubmitted.First().Name));
            Assert.That(viewModel.TrusteeData.FromSubmittedApplication.First().MonthYearOfBirth, Is.EqualTo(trusteesFromSubmitted.First().MonthYearOfBirth));
        }

        [Test]
        public void check_people_in_control_whos_in_control_details_from_submitted_are_returned()
        {
            var whosInControlFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ WhosInControlPostfix + SourceSubmitted,
                    MonthYearOfBirth = PersonInControlDob + WhosInControlPostfix + SourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetWhosInControlFromSubmitted(_applicationId)).ReturnsAsync(whosInControlFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.That(1, Is.EqualTo(viewModel.WhosInControlData.FromSubmittedApplication.Count));
            Assert.That(viewModel.WhosInControlData.FromSubmittedApplication.First().Name, Is.EqualTo(whosInControlFromSubmitted.First().Name));
            Assert.That(viewModel.WhosInControlData.FromSubmittedApplication.First().MonthYearOfBirth, Is.EqualTo(whosInControlFromSubmitted.First().MonthYearOfBirth));
        }
    }
}
