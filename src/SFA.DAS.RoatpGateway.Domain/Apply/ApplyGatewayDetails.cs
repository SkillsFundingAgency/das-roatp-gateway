using System;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class ApplyGatewayDetails
    {
        public DateTime? SourcesCheckedOn { get; set; }
        public string Comments { get; set; }
        public string ExternalComments { get; set; }
        public DateTime? OutcomeDateTime { get; set; }
        public string GatewaySubcontractorDeclarationClarificationUpload { get; set; }
    }
}
