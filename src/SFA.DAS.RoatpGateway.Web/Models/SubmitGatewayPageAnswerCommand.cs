using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.RoatpGateway.Web.Models
{
    public class SubmitGatewayPageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }
        public string OriginalStatus { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public string OptionClarificationText { get; set; }
        public string ClarificationAnswer { get; set; }
        public string ClarificationFile { get; set; }
        public string GatewayReviewStatus { get; set; }
        public IFormFileCollection ClarificationFileUploads { get; set; }
        public SubmitGatewayPageAnswerCommand()
        {

        }

        public SubmitGatewayPageAnswerCommand(RoatpGatewayPageViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            Status = viewModel.Status;
            OriginalStatus = viewModel.OriginalStatus;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
            OptionClarificationText = viewModel.OptionClarificationText;
            ClarificationAnswer = viewModel.ClarificationAnswer;
        }
    }
}

