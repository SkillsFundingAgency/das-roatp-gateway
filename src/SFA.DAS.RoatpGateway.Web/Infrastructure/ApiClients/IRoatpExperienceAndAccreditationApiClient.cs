using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpExperienceAndAccreditationApiClient
    {
        Task<SubcontractorDeclaration> GetSubcontractorDeclaration(Guid applicationId);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFile(Guid applicationId);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFileClarification(Guid applicationId, string fileName);
        Task<string> GetOfficeForStudents(Guid applicationId);
        Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId);
        Task<OfstedDetails> GetOfstedDetails(Guid applicationId);
    }
}
