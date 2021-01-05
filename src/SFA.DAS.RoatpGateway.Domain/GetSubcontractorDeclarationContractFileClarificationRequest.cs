using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetSubcontractorDeclarationContractFileClarificationRequest
    {
        public Guid ApplicationId { get; }
        public string FileName { get; }
        public GetSubcontractorDeclarationContractFileClarificationRequest(Guid applicationId, string fileName)
        {
            ApplicationId = applicationId;
            FileName = fileName;
        }
    }
}