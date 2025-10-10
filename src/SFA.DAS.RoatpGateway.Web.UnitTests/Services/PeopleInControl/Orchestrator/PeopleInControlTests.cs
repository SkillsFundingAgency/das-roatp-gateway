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

            Assert.That(viewModel.PageId, Is.EqualTo(GatewayPageId));
            Assert.That(viewModel.ApplicationId, Is.EqualTo(_applicationId));
            Assert.That(viewModel.OptionFailText, Is.EqualTo(_commonDetails.Comments));
            Assert.That(viewModel.OptionInProgressText, Is.Null);
            Assert.That(viewModel.OptionClarificationText, Is.Null);
            Assert.That(viewModel.OptionPassText, Is.Null);
            Assert.That(viewModel.Status, Is.EqualTo(_commonDetails.Status));
            Assert.That(viewModel.Ukprn, Is.EqualTo(_commonDetails.Ukprn));
            Assert.That(viewModel.ApplyLegalName, Is.EqualTo(_commonDetails.LegalName));
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

            Assert.That(viewModel.CompanyDirectorsData.FromExternalSource.Count, Is.EqualTo(1));

            Assert.That(directorsFromCompaniesHouse.First().Name, Is.EqualTo(viewModel.CompanyDirectorsData.FromExternalSource.First().Name));
            Assert.That(directorsFromCompaniesHouse.First().MonthYearOfBirth, Is.EqualTo(viewModel.CompanyDirectorsData.FromExternalSource.First().MonthYearOfBirth));
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

            Assert.That(viewModel.CompanyDirectorsData.FromSubmittedApplication.Count, Is.EqualTo(1));
            Assert.That(directorsFromSubmitted.First().Name, Is.EqualTo(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().Name));
            Assert.That(directorsFromSubmitted.First().MonthYearOfBirth, Is.EqualTo(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().MonthYearOfBirth));
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

            Assert.That(viewModel.PscData.FromExternalSource.Count, Is.EqualTo(1));

            Assert.That(pscsFromCompaniesHouse.First().Name, Is.EqualTo(viewModel.PscData.FromExternalSource.First().Name));
            Assert.That(pscsFromCompaniesHouse.First().MonthYearOfBirth, Is.EqualTo(viewModel.PscData.FromExternalSource.First().MonthYearOfBirth));
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

            Assert.That(viewModel.PscData.FromSubmittedApplication.Count, Is.EqualTo(1));
            Assert.That(pcsFromSubmitted.First().Name, Is.EqualTo(viewModel.PscData.FromSubmittedApplication.First().Name));
            Assert.That(pcsFromSubmitted.First().MonthYearOfBirth, Is.EqualTo(viewModel.PscData.FromSubmittedApplication.First().MonthYearOfBirth));
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
            Assert.That(viewModel.TrusteeData.FromExternalSource.Count, Is.EqualTo(1));
            Assert.That(trusteesFromCharityCommission.First().Name, Is.EqualTo(viewModel.TrusteeData.FromExternalSource.First().Name));
            Assert.That(trusteesFromCharityCommission.First().MonthYearOfBirth, Is.EqualTo(viewModel.TrusteeData.FromExternalSource.First().MonthYearOfBirth));

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
            Assert.That(viewModel.TrusteeData.FromSubmittedApplication.Count, Is.EqualTo(1));

            Assert.That(trusteesFromSubmitted.First().Name, Is.EqualTo(viewModel.TrusteeData.FromSubmittedApplication.First().Name));
            Assert.That(trusteesFromSubmitted.First().MonthYearOfBirth, Is.EqualTo(viewModel.TrusteeData.FromSubmittedApplication.First().MonthYearOfBirth));
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

            Assert.That(viewModel.WhosInControlData.FromSubmittedApplication.Count, Is.EqualTo(1));
            Assert.That(whosInControlFromSubmitted.First().Name, Is.EqualTo(viewModel.WhosInControlData.FromSubmittedApplication.First().Name));
            Assert.That(whosInControlFromSubmitted.First().MonthYearOfBirth, Is.EqualTo(viewModel.WhosInControlData.FromSubmittedApplication.First().MonthYearOfBirth));
        }
    }
}
