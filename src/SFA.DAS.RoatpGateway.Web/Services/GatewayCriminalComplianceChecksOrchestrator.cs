using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Extensions;
using System;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public class GatewayCriminalComplianceChecksOrchestrator : IGatewayCriminalComplianceChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpGatewayCriminalComplianceChecksApiClient _criminalChecksApiClient;
        private readonly ILogger<GatewayCriminalComplianceChecksOrchestrator> _logger;

        public GatewayCriminalComplianceChecksOrchestrator(IRoatpApplicationApiClient applyApiClient,
                                                           IRoatpGatewayCriminalComplianceChecksApiClient criminalChecksApiClient,
                                                           ILogger<GatewayCriminalComplianceChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _criminalChecksApiClient = criminalChecksApiClient;
            _logger = logger;
        }

        public async Task<CriminalCompliancePageViewModel> GetCriminalComplianceCheckViewModel(GetCriminalComplianceCheckRequest request)
        {
            var model = new CriminalCompliancePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GetSequenceNumberForPage(request.PageId),
                                                    request.PageId,
                                                    request.UserId,
                                                    request.UserName,
                                                    GetCaptionForPage(request.PageId),
                                                    CriminalCompliancePageConfiguration.Headings[request.PageId],
                                                    NoSelectionErrorMessages.Errors[request.PageId]);

            return await PopulateViewModelWithQuestionDetails(request.ApplicationId, request.PageId, model);
        }

        private async Task<CriminalCompliancePageViewModel> PopulateViewModelWithQuestionDetails(Guid applicationId, string pageId, CriminalCompliancePageViewModel model)
        {
            _logger.LogInformation($"Retrieving criminal compliance details for application {applicationId} page {pageId}");

            var criminalComplianceCheckDetails = await _criminalChecksApiClient.GetCriminalComplianceQuestionDetails(applicationId, pageId);
            model.QuestionText = criminalComplianceCheckDetails.QuestionText;
            model.ComplianceCheckQuestionId = criminalComplianceCheckDetails.QuestionId;
            model.ComplianceCheckAnswer = criminalComplianceCheckDetails.Answer;
            model.FurtherInformationQuestionId = criminalComplianceCheckDetails.FurtherQuestionId;
            model.FurtherInformationAnswer = criminalComplianceCheckDetails.FurtherAnswer;

            return model;
        }

        private static string GetCaptionForPage(string gatewayPageId)
        {
            switch (gatewayPageId)
            {
                case GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency:
                    return RoatpGatewayConstants.Captions.OrganisationsCriminalAndComplianceChecks;
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt:
                    return RoatpGatewayConstants.Captions.PeopleInControlCriminalAndComplianceChecks;
                default:
                    return string.Empty;
            }
        }

        private static int GetSequenceNumberForPage(string gatewayPageId)
        {
            switch (gatewayPageId)
            {
                case GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody:
                case GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency:
                    return GatewaySequences.OrganisationCriminalComplianceChecks;
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.TeachingRegulations:
                case GatewayPageIds.CriminalComplianceWhosInControlChecks.SchoolManagement:
                    return GatewaySequences.PeopleInControlCriminalComplianceChecks;
                default:
                    return int.MinValue;
            }
        }
    }
}
