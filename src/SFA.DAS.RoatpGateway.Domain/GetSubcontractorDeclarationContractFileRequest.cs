using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GetSubcontractorDeclarationContractFileRequest
    {
        public Guid ApplicationId { get; }

        public GetSubcontractorDeclarationContractFileRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
