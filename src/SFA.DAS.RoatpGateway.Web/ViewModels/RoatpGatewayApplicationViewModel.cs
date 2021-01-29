using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain.Apply;
using System;
using System.Collections.Generic;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public bool HasOversightOutcome { get; }
        public string GatewayReviewStatus { get; set; }

        public string OptionAskClarificationText { get; set; }
        public string OptionFailedText { get; set; }
        public string OptionFailedExternalText { get; set; }
        public string OptionApprovedText { get; set; }
        public string OptionRejectedText { get; set; }
        public string OptionExternalRejectedText { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public bool IsGatewayApproved { get; set; }
        public bool IsClarificationsSelectedAndAllFieldsSet { get; set; }
        public List<GatewaySequence> Sequences { get; set; }
        public bool ReadyToConfirm { get; set; }

        public bool TwoInTwoMonthsPassed { get; set; }

        // Read only
        public string GatewayReviewComment { get; set; }
        public DateTime? GatewayOutcomeDateTime { get; set; }
        public string GatewayUserName { get; set; }

        public DateTime? ApplicationClosedOn { get; set; }
        public string ApplicationClosedBy { get; set; }
        public string ApplicationComments { get; set; }
        public string ApplicationExternalComments { get; set; }

        // Model On Error
        public bool IsInvalid { get; set; }

        public string ErrorTextGatewayReviewStatus { get; set; }
        public string ErrorTextAskClarification { get; set; }
        public string ErrorTextFailed { get; set; }
        public string ErrorTextExternalFailed { get; set; }
        public string ErrorTextApproved { get; set; }
        public string ErrorTextRejected { get; set; }
        public string ErrorTextExternalRejected { get; set; }
        public string RadioCheckedAskClarification { get; set; }
        public string RadioCheckedFailed { get; set; }
        public string RadioCheckedApproved { get; set; }
        public string RadioCheckedRejected { get; set; }
   
        public string CssFormGroupError { get; set; }
        public string CssOnErrorAskClarification { get; set; }
        public string CssOnErrorApproved { get; set; }
        public string CssOnErrorFailed { get; set; }
        public string CssOnErrorRejected { get; set; }
        public string ApplicationEmailAddress { get; set; }
        public string CssOnErrorExternalFailed { get; set; }
        public string CssOnErrorExternalRejected { get; set; }

        public RoatpGatewayApplicationViewModel()
        {

        }

        public RoatpGatewayApplicationViewModel(Apply application, ApplicationOversightDetails oversightDetails)
        {
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicationStatus = application.ApplicationStatus;
            HasOversightOutcome = oversightDetails.OversightStatus != OversightReviewStatus.None;
            GatewayReviewStatus = application.GatewayReviewStatus;

            GatewayUserName = application.GatewayUserName;

            if (application.GatewayReviewStatus == RoatpGateway.Domain.GatewayReviewStatus.Pass)
            {
                IsGatewayApproved = true;
            }
            else if (application.GatewayReviewStatus == RoatpGateway.Domain.GatewayReviewStatus.Fail)
            {
                IsGatewayApproved = false;
            }

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;

                if (application.ApplicationStatus == RoatpGateway.Domain.ApplicationStatus.Withdrawn)
                {
                    ApplicationClosedOn = application.ApplyData.ApplyDetails.ApplicationWithdrawnOn;
                    ApplicationClosedBy = application.ApplyData.ApplyDetails.ApplicationWithdrawnBy;
                }
                else if (application.ApplicationStatus == RoatpGateway.Domain.ApplicationStatus.Removed)
                {
                    ApplicationClosedOn = application.ApplyData.ApplyDetails.ApplicationRemovedOn;
                    ApplicationClosedBy = application.ApplyData.ApplyDetails.ApplicationRemovedBy;
                }
            }

            if (application.ApplyData?.GatewayReviewDetails != null)
            {
                GatewayOutcomeDateTime = application.ApplyData.GatewayReviewDetails.OutcomeDateTime;
                GatewayReviewComment = application.ApplyData.GatewayReviewDetails.Comments;
            }

            ApplicationComments = application.Comments;
            ApplicationExternalComments = application.ExternalComments;
        }
    }

    public class GatewaySequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<GatewaySection> Sections { get; set; }
    }

    public class GatewaySection
    {
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string HiddenText { get; set; }
        public string LinkTitle { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}
