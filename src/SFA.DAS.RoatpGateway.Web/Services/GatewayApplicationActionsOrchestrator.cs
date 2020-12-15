using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public class GatewayApplicationActionsOrchestrator : IGatewayApplicationActionsOrchestrator
    {
        private const string _commonDetailsPageId = GatewayPageIds.TwoInTwelveMonths;
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public GatewayApplicationActionsOrchestrator(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<RoatpWithdrawApplicationViewModel> GetWithdrawApplicationViewModel(Guid applicationId, string userName)
        {
            RoatpWithdrawApplicationViewModel viewModel = null;

            var commonDetails = await _applyApiClient.GetPageCommonDetails(applicationId, _commonDetailsPageId, userName);
            if (commonDetails != null)
            {
                viewModel = new RoatpWithdrawApplicationViewModel
                {
                    ApplicationId = commonDetails.ApplicationId,
                    ApplicationStatus = commonDetails.ApplicationStatus,
                    Ukprn = commonDetails.Ukprn,
                    ApplyLegalName = commonDetails.LegalName,
                    ApplicationRoute = commonDetails.ProviderRouteName,
                    ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn
                };
            }

            return viewModel;
        }

        public void ProcessWithdrawApplicationViewModelOnError(RoatpWithdrawApplicationViewModel viewModelOnError, ValidationResponse validationResponse)
        {
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModelOnError.ErrorMessages = validationResponse.Errors;

                foreach (var error in viewModelOnError.ErrorMessages)
                {
                    if (error.Field.Equals("ConfirmApplicationActionYes"))
                    {
                        viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                    }
                    else if (error.Field.Equals("OptionYesText"))
                    {
                        viewModelOnError.CssOnErrorOptionYesText = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                        viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                    }
                }
            }
        }

        public async Task<RoatpRemoveApplicationViewModel> GetRemoveApplicationViewModel(Guid applicationId, string userName)
        {
            RoatpRemoveApplicationViewModel viewModel = null;

            var commonDetails = await _applyApiClient.GetPageCommonDetails(applicationId, _commonDetailsPageId, userName);
            if (commonDetails != null)
            {
                viewModel = new RoatpRemoveApplicationViewModel
                {
                    ApplicationId = commonDetails.ApplicationId,
                    ApplicationStatus = commonDetails.ApplicationStatus,
                    Ukprn = commonDetails.Ukprn,
                    ApplyLegalName = commonDetails.LegalName,
                    ApplicationRoute = commonDetails.ProviderRouteName,
                    ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn
                };
            }

            return viewModel;
        }

        public void ProcessRemoveApplicationViewModelOnError(RoatpRemoveApplicationViewModel viewModelOnError, ValidationResponse validationResponse)
        {
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModelOnError.ErrorMessages = validationResponse.Errors;

                foreach (var error in viewModelOnError.ErrorMessages)
                {
                    if (error.Field.Equals("ConfirmApplicationActionYes"))
                    {
                        viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                    }
                    else if (error.Field.Equals("OptionYesText"))
                    {
                        viewModelOnError.CssOnErrorOptionYesText = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                        viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                    }
                    else if (error.Field.Equals("OptionYesTextExternal"))
                    {
                        viewModelOnError.CssOnErrorOptionYesTextExternal = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                        viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                    }
                }
            }
        }
    }
}

