using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public class GatewayExperienceAndAccreditationOrchestrator : IGatewayExperienceAndAccreditationOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpExperienceAndAccreditationApiClient _experienceAndAccreditationApiClient;
        private readonly ILogger<GatewayExperienceAndAccreditationOrchestrator> _logger;

        public GatewayExperienceAndAccreditationOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpExperienceAndAccreditationApiClient experienceAndAccreditationApiClient, ILogger<GatewayExperienceAndAccreditationOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _experienceAndAccreditationApiClient = experienceAndAccreditationApiClient;
            _logger = logger;
        }

        public async Task<SubcontractorDeclarationViewModel> GetSubcontractorDeclarationViewModel(GetSubcontractorDeclarationRequest request)
        {
            _logger.LogInformation($"Retrieving subcontractor declaration details for application {request.ApplicationId}");

            var model = new SubcontractorDeclarationViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, request.UserName,
                                                                                                RoatpGatewayConstants.Captions.ExperienceAndAccreditation,
                                                                                                RoatpGatewayConstants.Headings.SubcontractorDeclaration,
                                                                                                NoSelectionErrorMessages.Errors[GatewayPageIds.SubcontractorDeclaration]);

            var subcontractorDeclaration = await _experienceAndAccreditationApiClient.GetSubcontractorDeclaration(request.ApplicationId);

            var commonDetails = await _applyApiClient.GetPageCommonDetails(request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, request.UserName);
            model.ClarificationFile = commonDetails.GatewaySubcontractorDeclarationClarificationUpload;

            model.HasDeliveredTrainingAsSubcontractor = subcontractorDeclaration.HasDeliveredTrainingAsSubcontractor;
            model.ContractFileName = subcontractorDeclaration.ContractFileName;

            return model;
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(GetSubcontractorDeclarationContractFileRequest request)
        {
            return await _experienceAndAccreditationApiClient.GetSubcontractorDeclarationContractFile(request.ApplicationId);
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFileClarification(
            GetSubcontractorDeclarationContractFileClarificationRequest request)
        {
            return await _experienceAndAccreditationApiClient.GetSubcontractorDeclarationContractFileClarification(request.ApplicationId, request.FileName);
        }

        public async Task<OfficeForStudentsViewModel> GetOfficeForStudentsViewModel(GetOfficeForStudentsRequest request)
        {
            _logger.LogInformation($"Retrieving office for students details for application {request.ApplicationId}");

            var model = new OfficeForStudentsViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.OfficeForStudents, request.UserName, RoatpGatewayConstants.Captions.ExperienceAndAccreditation, RoatpGatewayConstants.Headings.OfficeForStudents, NoSelectionErrorMessages.Errors[GatewayPageIds.OfficeForStudents]);

            model.IsOrganisationFundedByOfficeForStudents = await _experienceAndAccreditationApiClient.GetOfficeForStudents(request.ApplicationId) == "Yes";

            return model;
        }

        public async Task<InitialTeacherTrainingViewModel> GetInitialTeacherTrainingViewModel(GetInitialTeacherTrainingRequest request)
        {
            _logger.LogInformation($"Retrieving initial teacher training details for application {request.ApplicationId}");

            var model = new InitialTeacherTrainingViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.InitialTeacherTraining, request.UserName, RoatpGatewayConstants.Captions.ExperienceAndAccreditation, RoatpGatewayConstants.Headings.InitialTeacherTraining, NoSelectionErrorMessages.Errors[GatewayPageIds.InitialTeacherTraining]);

            var initialTeacherTraining = await _experienceAndAccreditationApiClient.GetInitialTeacherTraining(request.ApplicationId);

            model.DoesOrganisationOfferInitialTeacherTraining = initialTeacherTraining.DoesOrganisationOfferInitialTeacherTraining;
            model.IsPostGradOnlyApprenticeship = initialTeacherTraining.IsPostGradOnlyApprenticeship;

            return model;
        }

        public async Task<OfstedDetailsViewModel> GetOfstedDetailsViewModel(GetOfstedDetailsRequest request)
        {
            _logger.LogInformation($"Retrieving ofsted details for application {request.ApplicationId}");

            var model = new OfstedDetailsViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.Ofsted, request.UserName, RoatpGatewayConstants.Captions.ExperienceAndAccreditation, RoatpGatewayConstants.Headings.Ofsted, NoSelectionErrorMessages.Errors[GatewayPageIds.Ofsted]);

            var ofstedDetails = await _experienceAndAccreditationApiClient.GetOfstedDetails(request.ApplicationId);

            model.FullInspectionApprenticeshipGrade = ofstedDetails.FullInspectionApprenticeshipGrade;
            model.FullInspectionOverallEffectivenessGrade = ofstedDetails.FullInspectionOverallEffectivenessGrade;
            model.GradeWithinTheLast3Years = ofstedDetails.GradeWithinTheLast3Years;
            model.HasHadFullInspection = ofstedDetails.HasHadFullInspection;
            model.HasHadMonitoringVisit = ofstedDetails.HasHadMonitoringVisit;
            model.HasHadShortInspectionWithinLast3Years = ofstedDetails.HasHadShortInspectionWithinLast3Years;
            model.HasMaintainedFullGradeInShortInspection = ofstedDetails.HasMaintainedFullGradeInShortInspection;
            model.HasMaintainedFundingSinceInspection = ofstedDetails.HasMaintainedFundingSinceInspection;
            model.ReceivedFullInspectionGradeForApprenticeships = ofstedDetails.ReceivedFullInspectionGradeForApprenticeships;
            model.Has2MonitoringVisitsGradedInadequate = ofstedDetails.Has2MonitoringVisitsGradedInadequate;
            model.HasMonitoringVisitGradedInadequateInLast18Months =
                ofstedDetails.HasMonitoringVisitGradedInadequateInLast18Months;
            return model;
        }
    }
}
