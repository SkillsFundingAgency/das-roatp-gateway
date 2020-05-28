﻿namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class OrganisationCriminalCompliancePageViewModel : RoatpGatewayPageViewModel
    {
        public string QuestionText { get; set; }

        public string ComplianceCheckQuestionId { get; set; }
        public string FurtherInformationQuestionId { get; set; }

        public string ComplianceCheckAnswer { get; set; }
        public string FurtherInformationAnswer { get; set; }

        public string PostBackAction { get; set; }
    }
}