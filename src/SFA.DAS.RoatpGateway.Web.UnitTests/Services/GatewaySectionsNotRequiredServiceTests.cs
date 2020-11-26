using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Services
{
    [TestFixture]
    public class GatewaySectionsNotRequiredServiceTests
    {
        private Mock<IRoatpApplicationApiClient> _apiClient;
        private Mock<ILogger<GatewaySectionsNotRequiredService>> _logger;
        private GatewaySectionsNotRequiredService _service;
        private RoatpGatewayApplicationViewModel _viewModel;
        private Guid _applicationId;
        private Mock<IRoatpExperienceAndAccreditationApiClient> _accreditationClient;
        private const string UserName = "GatewayUser";

        [SetUp]
        public void Before_each_test()
        {
            _applicationId = Guid.NewGuid();

            _apiClient = new Mock<IRoatpApplicationApiClient>();
            _accreditationClient = new Mock<IRoatpExperienceAndAccreditationApiClient>();
            _logger = new Mock<ILogger<GatewaySectionsNotRequiredService>>();
            _service = new GatewaySectionsNotRequiredService(_apiClient.Object, _accreditationClient.Object, _logger.Object);

            var application = new Apply();
            _viewModel = new RoatpGatewayApplicationViewModel(application)
            {
                Sequences = new List<GatewaySequence>
                {
                    new GatewaySequence
                    {
                        SequenceNumber = GatewaySequences.OrganisationChecks,
                        Sections = new List<GatewaySection>
                        {
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.TradingName,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.WebsiteAddress,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.OfficeForStudents,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.InitialTeacherTraining,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.Ofsted,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.SubcontractorDeclaration,
                                Status = SectionReviewStatus.New
                            }
                        }
                    },
                    new GatewaySequence
                    {
                        SequenceNumber = GatewaySequences.PeopleInControlCriminalComplianceChecks,
                        Sections = new List<GatewaySection>
                        {
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated,
                                Status = SectionReviewStatus.New
                            },
                            new GatewaySection
                            {
                                PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities,
                                Status = SectionReviewStatus.New
                            }
                        }
                    }
                }
            };
            var providerDetails = new ProviderDetails
            {
                VerificationDetails = new List<VerificationDetails>
                {
                    new VerificationDetails
                    {
                        VerificationAuthority = VerificationAuthorities.CompaniesHouseAuthority
                    }
                }
            };
            _apiClient.Setup(x => x.GetUkrlpDetails(_applicationId)).ReturnsAsync(providerDetails);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Not_required_set_for_trading_name_when_not_found_on_ukrlp(string tradingName)
        {
            _apiClient.Setup(x => x.GetTradingName(_applicationId)).ReturnsAsync(tradingName);

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var tradingNameSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.TradingName);

            tradingNameSection.Should().NotBeNull();
            tradingNameSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [Test]
        public async Task Not_required_not_set_for_trading_name_when_found_on_ukrlp()
        {
            _apiClient.Setup(x => x.GetTradingName(_applicationId)).ReturnsAsync("Trading name");

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var tradingNameSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                   .FirstOrDefault(sec => sec.PageId == GatewayPageIds.TradingName);

            tradingNameSection.Should().NotBeNull();
            tradingNameSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Not_required_set_for_website_when_not_found_on_ukrlp_or_manually_entered(string websiteAddress)
        {
            _apiClient.Setup(x => x.GetOrganisationWebsiteAddress(_applicationId)).ReturnsAsync(websiteAddress);

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var websiteSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.WebsiteAddress);

            websiteSection.Should().NotBeNull();
            websiteSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [Test]
        public async Task Not_required_not_set_for_website_when_found_on_ukrlp_or_manually_entered()
        {
            _apiClient.Setup(x => x.GetOrganisationWebsiteAddress(_applicationId)).ReturnsAsync("www.site.com");

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var websiteSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.WebsiteAddress);

            websiteSection.Should().NotBeNull();
            websiteSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [TestCase(ProviderTypes.Employer)]
        [TestCase(ProviderTypes.Supporting)]
        public async Task Not_required_set_for_office_for_students_if_employer_or_supporting_provider(int providerTypeId)
        {
            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var officeForStudentsSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.OfficeForStudents);

            officeForStudentsSection.Should().NotBeNull();
            officeForStudentsSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [Test]
        public async Task Not_required_not_set_for_office_for_students_if_main_and_answered_no()
        {
            _accreditationClient.Setup(x => x.GetOfficeForStudents(_applicationId)).ReturnsAsync("No");

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var officeForStudentsSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.OfficeForStudents);

            officeForStudentsSection.Should().NotBeNull();
            officeForStudentsSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [Test]
        public async Task Not_required_not_set_for_office_for_students_if_main_and_answered_yes()
        {
            _accreditationClient.Setup(x => x.GetOfficeForStudents(_applicationId)).ReturnsAsync("Yes");

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var officeForStudentsSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.OfficeForStudents);

            officeForStudentsSection.Should().NotBeNull();
            officeForStudentsSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [Test]
        public async Task Not_required_set_for_initial_teacher_training_if_supporting_provider()
        {
            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Supporting);

            var initialTeacherTrainingSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.InitialTeacherTraining);

            initialTeacherTrainingSection.Should().NotBeNull();
            initialTeacherTrainingSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [TestCase(ProviderTypes.Main)]
        [TestCase(ProviderTypes.Employer)]
        public async Task Not_required_not_set_for_initial_teacher_training_if_main_or_employer_and_answered_no(int providerTypeId)
        {
            _accreditationClient.Setup(x => x.GetInitialTeacherTraining(_applicationId)).ReturnsAsync(new InitialTeacherTraining { DoesOrganisationOfferInitialTeacherTraining = false });

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var initialTeacherTrainingSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.InitialTeacherTraining);

            initialTeacherTrainingSection.Should().NotBeNull();
            initialTeacherTrainingSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [TestCase(ProviderTypes.Main)]
        [TestCase(ProviderTypes.Employer)]
        public async Task Not_required_not_set_for_initial_teacher_training_if_main_or_employer_and_answered_yes(int providerTypeId)
        {
            _accreditationClient.Setup(x => x.GetInitialTeacherTraining(_applicationId)).ReturnsAsync(new InitialTeacherTraining { DoesOrganisationOfferInitialTeacherTraining = true });

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var initialTeacherTrainingSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.InitialTeacherTraining);

            initialTeacherTrainingSection.Should().NotBeNull();
            initialTeacherTrainingSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [Test]
        public async Task Not_required_set_for_ofsted_if_supporting_provider()
        {
            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Supporting);

            var ofstedSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.Ofsted);

            ofstedSection.Should().NotBeNull();
            ofstedSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [TestCase(ProviderTypes.Main)]
        [TestCase(ProviderTypes.Employer)]
        public async Task Not_required_not_set_for_ofsted_if_main_or_employer_provider_and_postgrad_only_answered_no(int providerTypeId)
        {
            _accreditationClient.Setup(x => x.GetInitialTeacherTraining(_applicationId)).ReturnsAsync(new InitialTeacherTraining { IsPostGradOnlyApprenticeship = false });

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var ofstedSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.Ofsted);

            ofstedSection.Should().NotBeNull();
            ofstedSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [TestCase(ProviderTypes.Main)]
        [TestCase(ProviderTypes.Employer)]
        public async Task Not_required_set_for_ofsted_if_main_or_employer_provider_and_postgrad_only_answered_yes(int providerTypeId)
        {
            _accreditationClient.Setup(x => x.GetInitialTeacherTraining(_applicationId)).ReturnsAsync(new InitialTeacherTraining { IsPostGradOnlyApprenticeship = true });

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var ofstedSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.Ofsted);

            ofstedSection.Should().NotBeNull();
            ofstedSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [TestCase(ProviderTypes.Main)]
        [TestCase(ProviderTypes.Employer)]
        public async Task Not_required_set_for_subcontractor_declaration_if_not_supporting_provider(int providerTypeId)
        {
            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, providerTypeId);

            var subcontractorSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.SubcontractorDeclaration);

            subcontractorSection.Should().NotBeNull();
            subcontractorSection.Status.Should().Be(SectionReviewStatus.NotRequired);
        }

        [Test]
        public async Task Not_required_not_set_for_subcontractor_declaration_if_supporting_provider()
        {
            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Supporting);

            var subcontractorSection = _viewModel.Sequences.SelectMany(seq => seq.Sections)
                    .FirstOrDefault(sec => sec.PageId == GatewayPageIds.SubcontractorDeclaration);

            subcontractorSection.Should().NotBeNull();
            subcontractorSection.Status.Should().Be(SectionReviewStatus.New);
        }

        [TestCase(VerificationAuthorities.CharityCommissionAuthority)]
        [TestCase(VerificationAuthorities.CompaniesHouseAuthority)]
        public async Task Not_required_not_set_for_people_in_control_criminal_compliance_if_company_or_charity(string verificationAuthority)
        {
            var providerDetails = new ProviderDetails
            {
                VerificationDetails = new List<VerificationDetails>
                {
                    new VerificationDetails
                    {
                        VerificationAuthority = verificationAuthority
                    }
                }
            };
            _apiClient.Setup(x => x.GetUkrlpDetails(_applicationId)).ReturnsAsync(providerDetails);

            await _service.SetupNotRequiredLinks(_applicationId, UserName, _viewModel, (int)ProviderTypes.Main);

            var peopleInControlCriminalComplianceSequence = _viewModel.Sequences.FirstOrDefault(x => x.SequenceNumber
                                                                                                == GatewaySequences.PeopleInControlCriminalComplianceChecks);

            peopleInControlCriminalComplianceSequence.Should().NotBeNull();
            peopleInControlCriminalComplianceSequence.Sections.Any(x => x.Status == SectionReviewStatus.NotRequired).Should().BeFalse();
        }

    }
}
