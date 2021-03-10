using System;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Models;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public class GatewayOverviewOrchestrator : IGatewayOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IGatewaySectionsNotRequiredService _sectionsNotRequiredService;

        public GatewayOverviewOrchestrator(IRoatpApplicationApiClient applyApiClient, IGatewaySectionsNotRequiredService sectionsNotRequiredService)
        {
            _applyApiClient = applyApiClient;
            _sectionsNotRequiredService = sectionsNotRequiredService;
        }

        public async Task<RoatpGatewayApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);

            if (application is null)
            {
                return null;
            }

            var oversightDetails = await _applyApiClient.GetOversightDetails(request.ApplicationId);

            var contact = await _applyApiClient.GetContactDetails(request.ApplicationId);
            var viewmodel = new RoatpGatewayApplicationViewModel(application, oversightDetails)
            {
                ApplicationEmailAddress  = contact?.Email,
                Sequences = GetCoreGatewayApplicationViewModel()
            };

            var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
            if (savedStatuses != null && !savedStatuses.Any())
            {
                var providerRoute = application.ApplyData.ApplyDetails.ProviderRoute;
                await _sectionsNotRequiredService.SetupNotRequiredLinks(request.ApplicationId, request.UserName, viewmodel, providerRoute);
            }
            else
            {
                foreach (var currentStatus in savedStatuses ?? new List<GatewayPageAnswerSummary>())
                {
                    // Inject the statuses and comments into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Status = currentStatus?.Status;
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Comment = currentStatus?.Comments;
                }
            }

            var sections = viewmodel.Sequences.SelectMany(seq => seq.Sections);
            viewmodel.IsClarificationsSelectedAndAllFieldsSet = IsAskForClarificationActive(sections);
            viewmodel.OneInTwoMonthsPassed = OneInTwelveMonthsPassed(sections);
            viewmodel.ReadyToConfirm = IsReadyToConfirm(sections);

            return viewmodel;
        }

        public async Task<RoatpGatewayClarificationsViewModel> GetClarificationViewModel(GetApplicationClarificationsRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            if (application is null)
            {
                return null;
            }

            var contact = await _applyApiClient.GetContactDetails(request.ApplicationId);

            var viewmodel = new RoatpGatewayClarificationsViewModel(application)
            {
                ApplicationEmailAddress = contact?.Email
            };

            viewmodel.Sequences = await ConstructClarificationSequences(request.ApplicationId);
            return viewmodel;
        }

        public async Task<RoatpGatewayApplicationViewModel> GetConfirmOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);

            if (application is null)
            {
                return null;
            }

            var oversightDetails = await _applyApiClient.GetOversightDetails(request.ApplicationId);

            var viewmodel = new RoatpGatewayApplicationViewModel(application, oversightDetails)
            {
                Sequences = GetCoreGatewayApplicationViewModel()
            };

            var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
            if (savedStatuses != null && !savedStatuses.Any())
            {
                viewmodel.ReadyToConfirm = false;
                return viewmodel;
            }
            else
            {
                foreach (var currentStatus in savedStatuses ?? new List<GatewayPageAnswerSummary>())
                {
                    // Inject the statuses and comments into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Status = currentStatus?.Status;
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Comment = currentStatus?.Comments;
                }
            }

            var sections = viewmodel.Sequences.SelectMany(seq => seq.Sections);
            viewmodel.IsClarificationsSelectedAndAllFieldsSet = sections.Any(x => x.Status == SectionReviewStatus.Clarification);
            viewmodel.OneInTwoMonthsPassed = OneInTwelveMonthsPassed(sections);
            viewmodel.ReadyToConfirm = IsReadyToConfirm(sections);

            return viewmodel;
        }

        public void ProcessViewModelOnError(RoatpGatewayApplicationViewModel viewModelOnError, RoatpGatewayApplicationViewModel viewModel, ValidationResponse validationResponse)
        {
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModelOnError.IsInvalid = true;
                viewModelOnError.ErrorMessages = validationResponse.Errors;
                viewModelOnError.GatewayReviewStatus = viewModel.GatewayReviewStatus;
                viewModelOnError.OptionAskClarificationText = viewModel.OptionAskClarificationText;
                viewModelOnError.OptionFailedText = viewModel.OptionFailedText;
                viewModelOnError.OptionFailedExternalText = viewModel.OptionFailedExternalText;
                viewModelOnError.OptionApprovedText = viewModel.OptionApprovedText;
                viewModelOnError.OptionRejectedText = viewModel.OptionRejectedText;
                viewModelOnError.OptionExternalRejectedText = viewModel.OptionExternalRejectedText;
               
                viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                viewModelOnError.RadioCheckedAskClarification = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                viewModelOnError.RadioCheckedFailed = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.Fail ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                viewModelOnError.RadioCheckedApproved = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.Pass ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                viewModelOnError.RadioCheckedRejected = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.Reject ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                foreach (var error in viewModelOnError.ErrorMessages)
                {
                    if (error.Field.Equals(nameof(viewModelOnError.GatewayReviewStatus)))
                    {
                        viewModelOnError.ErrorTextGatewayReviewStatus = error.ErrorMessage;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionAskClarificationText)))
                    {
                        viewModelOnError.ErrorTextAskClarification = error.ErrorMessage;
                        viewModelOnError.CssOnErrorAskClarification = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionFailedText)))
                    {
                        viewModelOnError.ErrorTextFailed = error.ErrorMessage;
                        viewModelOnError.CssOnErrorFailed = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionFailedExternalText)))
                    {
                        viewModelOnError.ErrorTextExternalFailed = error.ErrorMessage;
                        viewModelOnError.CssOnErrorExternalFailed = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionApprovedText)))
                    {
                        viewModelOnError.ErrorTextApproved = error.ErrorMessage;
                        viewModelOnError.CssOnErrorApproved = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionRejectedText)))
                    {
                        viewModelOnError.ErrorTextRejected = error.ErrorMessage;
                        viewModelOnError.CssOnErrorRejected = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionExternalRejectedText)))
                    {
                        viewModelOnError.ErrorTextExternalRejected = error.ErrorMessage;
                        viewModelOnError.CssOnErrorExternalRejected = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }
                }
            }
        }

        private async Task<List<ClarificationSequence>> ConstructClarificationSequences(Guid applicationId)
        {
            var clarificationSequences = new List<ClarificationSequence>();
            var coreSequences = GetCoreGatewayApplicationViewModel();

            var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(applicationId);

            foreach (var sequence in coreSequences.OrderBy(x => x.SequenceNumber))
            {
                foreach (var section in sequence.Sections.OrderBy(x => x.SectionNumber))
                {
                    foreach (var status in savedStatuses.Where(x => x.Status == SectionReviewStatus.Clarification))
                    {
                        if (section.PageId != status.PageId) continue;

                        if (clarificationSequences.All(x => x.SequenceNumber != sequence.SequenceNumber))
                            clarificationSequences.Add(new ClarificationSequence
                            {
                                Sections = new List<ClarificationSection>(),
                                SequenceNumber = sequence.SequenceNumber,
                                SequenceTitle = sequence.SequenceTitle
                            });

                        clarificationSequences.FirstOrDefault(x => x.SequenceNumber == sequence.SequenceNumber)
                            ?.Sections.Add(new ClarificationSection { PageTitle = section.LinkTitle, Comment = status.Comments });

                        if (status.PageId == GatewayPageIds.OneInTwelveMonths)
                            return clarificationSequences;
                    }
                }
            }

            return clarificationSequences;
        }

        private static bool OneInTwelveMonthsPassed(IEnumerable<GatewaySection> sections)
        {
            return sections.Where(sec => sec.PageId == GatewayPageIds.OneInTwelveMonths).Any(sec => sec.Status == SectionReviewStatus.Pass);
        }

        private static bool IsReadyToConfirm(IEnumerable<GatewaySection> sections)
        {
            var isReadyToConfirm = true;

            var oneInTwelveMonthsFailed = sections.Where(sec => sec.PageId == GatewayPageIds.OneInTwelveMonths).Any(sec => sec.Status == SectionReviewStatus.Fail);

            if (!oneInTwelveMonthsFailed)
            {
                var gradedStatutes = new[] { SectionReviewStatus.Pass, SectionReviewStatus.Fail, SectionReviewStatus.NotRequired };

                foreach (var section in sections)
                {
                    if (section.Status is null || !gradedStatutes.Contains(section.Status))
                    {
                        isReadyToConfirm = false;
                        break;
                    }
                }
            }

            return isReadyToConfirm;
        }

        private static bool IsAskForClarificationActive(IEnumerable<GatewaySection> sections)
        {
            var clarificationsPresent= sections.Any(x => x.Status == SectionReviewStatus.Clarification);

            if (!clarificationsPresent)
                return false;

            var oneInTwelveMonthClarification = sections.Where(sec => sec.PageId == GatewayPageIds.OneInTwelveMonths).Any(sec => sec.Status == SectionReviewStatus.Clarification);
            
            if (oneInTwelveMonthClarification)
                return true;

            var gradedStatutes = new[] { SectionReviewStatus.Pass, SectionReviewStatus.Fail, SectionReviewStatus.NotRequired, SectionReviewStatus.Clarification };

            foreach (var section in sections)
            {
                if (section.Status is null || !gradedStatutes.Contains(section.Status))
                {
                    return false;
                }
            }

            return true;

        }

        // APR-1467 Code Stubbed Data
        private static List<GatewaySequence> GetCoreGatewayApplicationViewModel()
        {
            return new List<GatewaySequence>
            {
                new GatewaySequence
                {
                    SequenceNumber = 1,
                    SequenceTitle = "Organisation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.OneInTwelveMonths,  LinkTitle = "1 application in 12 months" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.LegalName,  LinkTitle = "Legal name" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.TradingName, LinkTitle = "Trading name" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.OrganisationStatus, LinkTitle = "Organisation status" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.Address, LinkTitle = "Address" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.IcoNumber, LinkTitle = "ICO registration number" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.WebsiteAddress,  LinkTitle = "Website address" },
                        new GatewaySection { SectionNumber = 8, PageId = GatewayPageIds.OrganisationRisk,  LinkTitle = "Organisation high risk" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 2,
                    SequenceTitle = "People in control checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.PeopleInControl, LinkTitle = "People in control", HiddenText = "for people in control checks" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.PeopleInControlRisk,   LinkTitle = "People in control high risk" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 3,
                    SequenceTitle = "Register checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.Roatp, LinkTitle = "RoATP" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.Roepao,  LinkTitle = "Register of end-point assessment organisations" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 4,
                    SequenceTitle = "Experience and accreditation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.OfficeForStudents,  LinkTitle = "Office for Student (OfS)" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.InitialTeacherTraining, LinkTitle = "Initial teacher training (ITT)" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.Ofsted,  LinkTitle = "Ofsted" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.SubcontractorDeclaration, LinkTitle = "Subcontractor declaration" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 5,
                    SequenceTitle = "Organisation's criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors,  LinkTitle = "Composition with creditors" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, LinkTitle = "Failed to pay back funds", HiddenText = "for the organisation" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination,  LinkTitle = "Contract terminated early by a public body", HiddenText = "for the organisation" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the organisation" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, LinkTitle = "Register of Training Organisations (RoTO)" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, LinkTitle = "Funding removed from any education bodies" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, LinkTitle = "Removed from any professional or trade registers" },
                        new GatewaySection { SectionNumber = 8, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation,  LinkTitle = "Initial Teacher Training accreditation" },
                        new GatewaySection { SectionNumber = 9, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, LinkTitle = "Removed from any charity register" },
                        new GatewaySection { SectionNumber = 10, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding,  LinkTitle = "Investigated due to safeguarding issues" },
                        new GatewaySection { SectionNumber = 11, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing,  LinkTitle = "Investigated due to whistleblowing issues" },
                        new GatewaySection { SectionNumber = 12, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, LinkTitle = "Insolvency or winding up proceedings" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 6,
                    SequenceTitle = "People in control's criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions,  LinkTitle = "Unspent criminal convictions" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, LinkTitle = "Failed to pay back funds", HiddenText = "for the people in control" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, LinkTitle = "Investigated for fraud or irregularities" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation,  LinkTitle = "Ongoing investigations for fraud or irregularities" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, LinkTitle = "Contract terminated early by a public body", HiddenText = "for the people in control" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract,  LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the people in control" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments,  LinkTitle = "Breached tax payments or social security contributions" },
                        new GatewaySection { SectionNumber = 8, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, LinkTitle = "Register of Removed Trustees" },
                        new GatewaySection { SectionNumber = 9, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt,  LinkTitle = "Been made bankrupt" }
                    }
                }
            };
        }
    }
}

