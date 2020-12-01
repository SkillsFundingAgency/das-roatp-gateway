using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Extensions
{
    internal static class RoatpGatewayPageViewModelExtensions
    {
        internal static async Task PopulatePageCommonDetails(this RoatpGatewayPageViewModel viewModel, IRoatpApplicationApiClient applyApiClient, Guid applicationId, string pageId, string userName,
            string caption, string heading, string noSelectionErrorMessage)
        {
            var commonDetails = await applyApiClient.GetPageCommonDetails(applicationId, pageId, userName);

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = pageId;
            viewModel.ApplyLegalName = commonDetails.LegalName;
            viewModel.ApplicationRoute = commonDetails.ProviderRouteName;
            viewModel.Ukprn = commonDetails.Ukprn;
            viewModel.Status = commonDetails.Status;
            viewModel.OptionPassText = commonDetails.OptionPassText;
            viewModel.OptionFailText = commonDetails.OptionFailText;
            viewModel.OptionInProgressText = commonDetails.OptionInProgressText;
            viewModel.SourcesCheckedOn = commonDetails.CheckedOn;
            viewModel.ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn;
            viewModel.Caption = caption;
            viewModel.Heading = heading;
            viewModel.NoSelectionErrorMessage = noSelectionErrorMessage;
        }
    }
}
