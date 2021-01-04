using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayPageViewModel
    {
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }
        public string GatewayReviewStatus { get; set; }

        public string Status { get; set; }

        public string Ukprn { get; set; }
        public string ApplyLegalName { get; set; }
        public string ApplicationRoute { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public string OptionClarificationText { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }


        public DateTime? SourcesCheckedOn { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }

        public DateTime? GatewayOutcomeMadeOn { get; set; }
        public string GatewayOutcomeMadeBy { get; set; }
        public DateTime? OutcomeMadeOn { get; set; }
        public string OutcomeMadeBy { get; set; }
        public string Comments { get; set; }

        public string Heading { get; set; }
        public string Caption { get; set; }
        public string NoSelectionErrorMessage { get; set; }
    
        public string ClarificationAnswer { get; set; }
        public string ClarificationComments { get; set; }
        public string ClarificationBy { get; set; }
        public DateTime? ClarificationDate { get; set; }
    }
}
