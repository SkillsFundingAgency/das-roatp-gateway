using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Extensions
{
    internal static class RoatpGatewayPageViewModelExtensions
    {
        internal static async Task PopulatePageCommonDetails(this RoatpGatewayPageViewModel viewModel, IRoatpApplicationApiClient applyApiClient, Guid applicationId,
            int sequenceNumber, string pageId, string userId, string userName, string caption, string heading, string noSelectionErrorMessage)
        {
            GatewayCommonDetails commonDetails;
            try
            {
                commonDetails = await applyApiClient.GetPageCommonDetails(applicationId, pageId, userId, userName);
            }
            catch (Exception ex)
            {
                throw new ExternalApiException("An error occurred when retrieving Gateway common details from apply", ex);
            }

            if (commonDetails == null)
            {
                throw new ExternalApiException("An error occurred when retrieving Gateway common details from apply, null value returned");
            }

            viewModel.ApplicationId = commonDetails.ApplicationId;
            viewModel.SequenceNumber = sequenceNumber;
            viewModel.PageId = commonDetails.PageId;
            viewModel.GatewayReviewStatus = commonDetails.GatewayReviewStatus;
            viewModel.ApplyLegalName = commonDetails.LegalName;
            viewModel.ApplicationRoute = commonDetails.ProviderRouteName;
            viewModel.Ukprn = commonDetails.Ukprn;
            viewModel.Status = commonDetails.Status;
            viewModel.OriginalStatus = commonDetails.Status;
            viewModel.SourcesCheckedOn = commonDetails.SourcesCheckedOn;
            viewModel.ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn;
            viewModel.Caption = caption;
            viewModel.Heading = heading;
            viewModel.NoSelectionErrorMessage = noSelectionErrorMessage;
            viewModel.ApplicationStatus = commonDetails.ApplicationStatus;
            viewModel.GatewayOutcomeMadeOn = commonDetails.GatewayOutcomeMadeOn;
            viewModel.GatewayOutcomeMadeBy = commonDetails.GatewayOutcomeMadeBy;
            viewModel.OutcomeMadeOn = commonDetails.OutcomeMadeOn;
            viewModel.OutcomeMadeBy = commonDetails.OutcomeMadeBy;
            viewModel.Comments = commonDetails.Comments;
            viewModel.ClarificationComments = commonDetails.ClarificationComments;
            viewModel.ClarificationBy = commonDetails.ClarificationBy;
            viewModel.ClarificationDate = commonDetails.ClarificationDate;
            viewModel.ClarificationAnswer = commonDetails.ClarificationAnswer;

            switch (commonDetails.Status)
            {
                case SectionReviewStatus.Pass:
                    viewModel.OptionPassText = commonDetails.Comments;
                    break;
                case SectionReviewStatus.InProgress:
                    viewModel.OptionInProgressText = commonDetails.Comments;
                    break;
                case SectionReviewStatus.Fail:
                    viewModel.OptionFailText = commonDetails.Comments;
                    break;
                case SectionReviewStatus.Clarification:
                    viewModel.OptionClarificationText = commonDetails.Comments;
                    break;
            }
        }
    }
}
