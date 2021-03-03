using System;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayOutcomeViewModel
    {
        public Guid ApplicationId { get; set; }
        public string Ukprn { get; set; }
        public string ApplyLegalName { get; set; }
        public string ApplicationRoute { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public string ApplicationEmailAddress { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }
        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string GatewayReviewComment { get; set; }
        public string GatewayReviewExternalComment { get; set; }

        public string CssFormGroupError { get; set; }
    }
}