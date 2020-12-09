using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain.Apply;
using System;
using System.Collections.Generic;
using SFA.DAS.RoatpGateway.Web.Models;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayClarificationsViewModel : OrganisationDetailsViewModel
    {
        // public Guid Id { get; }
        public Guid ApplicationId { get; set; }
        // public Guid OrgId { get; }
        //
        // public string ApplicationStatus { get; }
        // public string GatewayReviewStatus { get; set; }
        //
        //
        //
        //
         public string ConfirmAskForClarification { get; set; }


        // public string OptionAskClarificationText { get; set; }
        // public string OptionFailedText { get; set; }
        // public string OptionApprovedText { get; set; }
        // public string OptionRejectedText { get; set; }
        // public string GatewayReviewComment { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        // public bool IsGatewayApproved { get; set; }
        // public bool IsClarificationsSelectedAndAllFieldsSet { get; set; }
        public List<ClarificationSequence> Sequences { get; set; }
        // public bool ReadyToConfirm { get; set; }
        //
        // public bool TwoInTwoMonthsPassed { get; set; }
        //
        // // Model On Error
        // public bool IsInvalid { get; set; }
        //
        // public string ErrorTextGatewayReviewStatus { get; set; }
        // public string ErrorTextAskClarification { get; set; }
        // public string ErrorTextFailed { get; set; }
        // public string ErrorTextApproved { get; set; }
        // public string ErrorTextRejected { get; set; }
        // public string RadioCheckedAskClarification { get; set; }
        // public string RadioCheckedFailed { get; set; }
        // public string RadioCheckedApproved { get; set; }
        // public string RadioCheckedRejected { get; set; }
   
        public string CssFormGroupError { get; set; }
        // public string CssOnErrorAskClarification { get; set; }
        // public string CssOnErrorApproved { get; set; }
        // public string CssOnErrorFailed { get; set; }
        // public string CssOnErrorRejected { get; set; }
        public string ApplicationEmailAddress { get; set; }

        public RoatpGatewayClarificationsViewModel()
        {

        }

        public RoatpGatewayClarificationsViewModel(Apply application)
        {
            //Id = application.Id;
            ApplicationId = application.ApplicationId;
            //OrgId = application.OrganisationId;
        
           // ApplicationStatus = application.ApplicationStatus;
           // GatewayReviewStatus = application.GatewayReviewStatus;
        
            // if (application.GatewayReviewStatus == RoatpGateway.Domain.GatewayReviewStatus.Pass)
            // {
            //     IsGatewayApproved = true;
            // }
            // else if (application.GatewayReviewStatus == RoatpGateway.Domain.GatewayReviewStatus.Fail)
            // {
            //     IsGatewayApproved = false;
            // }
        
            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }
        }
    }
}
