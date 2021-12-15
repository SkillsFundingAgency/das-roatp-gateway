using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Services;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services.PeopleInControl.Orchestrator
{
    [TestFixture]
    public class PeopleInControlHighRiskTests
    {
        private PeopleInControlOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private Mock<ILogger<PeopleInControlOrchestrator>> _logger;
        private Mock<IOuterApiClient> _roatpApiClient;

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
        private readonly string _charityNumber = "111233323";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _roatpApiClient = new Mock<IOuterApiClient>();

            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _logger = new Mock<ILogger<PeopleInControlOrchestrator>>();
            _orchestrator = new PeopleInControlOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _roatpApiClient.Object, _logger.Object);

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
            _organisationSummaryApiClient.Setup(x => x.GetCharityNumber(_applicationId)).ReturnsAsync(_charityNumber);
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, GatewayPageId, UserId, UserName)).ReturnsAsync(_commonDetails);
        }

        [Test]
        public void check_people_in_control_high_risk_common_details_are_returned()
        {

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageId, viewModel.PageId);
            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(_commonDetails.Comments, viewModel.OptionFailText);
            Assert.Null(viewModel.OptionInProgressText);
            Assert.Null(viewModel.OptionClarificationText);
            Assert.Null(viewModel.OptionPassText);
            Assert.AreEqual(_commonDetails.Status, viewModel.Status);
            Assert.AreEqual(_commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(_commonDetails.LegalName, viewModel.ApplyLegalName);
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

            Assert.AreEqual(1, viewModel.CompanyDirectorsData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.CompanyDirectorsData.PeopleInControl.First().Name, directorsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.PeopleInControl.First().MonthYearOfBirth, directorsFromSubmitted.First().MonthYearOfBirth);
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

            Assert.AreEqual(1, viewModel.PscData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.PscData.PeopleInControl.First().Name, pcsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.PscData.PeopleInControl.First().MonthYearOfBirth, pcsFromSubmitted.First().MonthYearOfBirth);
        }

         [Test]
         public void check_people_in_control_trustee_details_from_submitted_are_returned()
         {
             var name = PersonInControlName + TrusteesPostfix;

             var trusteesFromSource = new List<TrusteeInformation>
             {
                 new TrusteeInformation { Id = "1", Name = name}
             };

             _roatpApiClient.Setup(x => x.GetCharityDetails(_charityNumber)).ReturnsAsync(new CharityDetails { Trustees = trusteesFromSource });


            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserId, UserName);
             var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);
        
             var viewModel = response.Result;
             Assert.AreEqual(1, viewModel.TrusteeData.PeopleInControl.Count);
        
             Assert.AreEqual(viewModel.TrusteeData.PeopleInControl.First().Name, name);
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

            Assert.AreEqual(1, viewModel.WhosInControlData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.WhosInControlData.PeopleInControl.First().Name, whosInControlFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.WhosInControlData.PeopleInControl.First().MonthYearOfBirth, whosInControlFromSubmitted.First().MonthYearOfBirth);
        }
    }
}
