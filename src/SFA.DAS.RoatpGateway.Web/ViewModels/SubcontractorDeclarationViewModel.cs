using Microsoft.AspNetCore.Http;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class SubcontractorDeclarationViewModel : RoatpGatewayPageViewModel
    {
        public bool HasDeliveredTrainingAsSubcontractor { get; set; }

        public string ContractFileName { get; set; }

        public string ClarificationFile { get; set; }
        public IFormFileCollection FilesToUpload { get; set; }
    }
}
