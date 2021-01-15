using System;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayOutcomeViewModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string GatewayReviewComment { get; set; }
        public string GatewayReviewExternalComment { get; set; }

        public string CssFormGroupError { get; set; }
    }
}