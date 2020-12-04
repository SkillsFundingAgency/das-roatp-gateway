using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain.Apply;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public string GatewayReviewStatus { get; set; }
        public string OptionAskClarificationText { get; set; }
        public string OptionFailedText { get; set; }
        public string OptionApprovedText { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public bool IsGatewayApproved { get; set; }

        public List<GatewaySequence> Sequences { get; set; }
        public bool ReadyToConfirm { get; set; }

        public bool TwoInTwoMonthsPassed { get; set; }

        // Read only
        public string GatewayReviewComments { get; set; }
        public DateTime? GatewayOutcomeDateTime { get; set; }
        public string GatewayUserName { get; set; }

        // Model On Error
        public bool IsInvalid { get; set; }

        public string ErrorTextGatewayReviewStatus { get; set; }
        public string ErrorTextAskClarification { get; set; }
        public string ErrorTextFailed { get; set; }
        public string ErrorTextApproved { get; set; }

        public string RadioCheckedAskClarification { get; set; }
        public string RadioCheckedFailed { get; set; }
        public string RadioCheckedApproved { get; set; }

   
        public string CssFormGroupError { get; set; }
        public string CssOnErrorAskClarification { get; set; }
        public string CssOnErrorApproved { get; set; }
        public string CssOnErrorFailed { get; set; }
        public string ApplicationEmailAddress { get; set; }

        public RoatpGatewayApplicationViewModel()
        {

        }

        public RoatpGatewayApplicationViewModel(Apply application)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicationStatus = application.ApplicationStatus;
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
            }

            if (application.ApplyData?.GatewayReviewDetails != null)
            {
                GatewayOutcomeDateTime = application.ApplyData.GatewayReviewDetails.OutcomeDateTime;
                GatewayReviewComments = application.ApplyData.GatewayReviewDetails.Comments;
            }
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
