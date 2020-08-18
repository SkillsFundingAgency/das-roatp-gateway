using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;

namespace SFA.DAS.RoatpGateway.Web.Models
{
    public class SubmitGatewayPageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }

        public SubmitGatewayPageAnswerCommand()
        {

        }

        public SubmitGatewayPageAnswerCommand(RoatpGatewayPageViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            Status = viewModel.Status;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
        }
    }
}

