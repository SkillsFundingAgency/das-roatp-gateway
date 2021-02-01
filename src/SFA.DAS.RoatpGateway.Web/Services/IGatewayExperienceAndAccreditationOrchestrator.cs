using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewayExperienceAndAccreditationOrchestrator
    {
        Task<SubcontractorDeclarationViewModel> GetSubcontractorDeclarationViewModel(GetSubcontractorDeclarationRequest subcontractorDeclarationRequest);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFile(GetSubcontractorDeclarationContractFileRequest subcontractorDeclarationRequest);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFileClarification(GetSubcontractorDeclarationContractFileClarificationRequest request);
        Task<OfficeForStudentsViewModel> GetOfficeForStudentsViewModel(GetOfficeForStudentsRequest request);
        Task<OfstedDetailsViewModel> GetOfstedDetailsViewModel(GetOfstedDetailsRequest request);
        Task<InitialTeacherTrainingViewModel> GetInitialTeacherTrainingViewModel(GetInitialTeacherTrainingRequest request);
    }
}
