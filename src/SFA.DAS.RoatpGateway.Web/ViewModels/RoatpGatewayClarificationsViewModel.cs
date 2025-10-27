using SFA.DAS.RoatpGateway.Domain.Apply;
using System;
using System.Collections.Generic;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayClarificationsViewModel : OrganisationDetailsViewModel
    {

        public Guid ApplicationId { get; set; }

        public string GatewayReviewStatus { get; set; }

        public string ConfirmAskForClarification { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }
        public List<ClarificationSequence> Sequences { get; set; }

        public string CssFormGroupError { get; set; }
        public string ApplicationEmailAddress { get; set; }

        public RoatpGatewayClarificationsViewModel()
        {
        }

        public RoatpGatewayClarificationsViewModel(Apply application)
        {
            ApplicationId = application.ApplicationId;

            GatewayReviewStatus = application.GatewayReviewStatus;

            if (application.ApplyData?.ApplyDetails == null) return;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;
            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
        }
    }
}
