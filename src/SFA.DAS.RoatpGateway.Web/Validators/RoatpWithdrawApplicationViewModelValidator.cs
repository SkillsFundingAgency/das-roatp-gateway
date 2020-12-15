using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpWithdrawApplicationViewModelValidator : IRoatpWithdrawApplicationViewModelValidator
    {
        private const string NoSelectionErrorMessage = "Select if you are sure the applicant wants to withdraw their application";
        private const string ErrorEnterComments = "Enter internal comments";
        private const string TooManyWords = "Your internal comments must be 150 words or less";

        public async Task<ValidationResponse> Validate(RoatpWithdrawApplicationViewModel viewModel)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrEmpty(viewModel.ConfirmApplicationAction))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("ConfirmApplicationActionYes", NoSelectionErrorMessage));
            }
            else if (viewModel.ConfirmApplicationAction.Equals(HtmlAndCssElements.RadioButtonValueYes, StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(viewModel.OptionYesText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionYesText", ErrorEnterComments));
                }
                else
                {
                    var wordCount = viewModel.OptionYesText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionYesText", TooManyWords));
                    }
                }
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IRoatpWithdrawApplicationViewModelValidator
    {
        Task<ValidationResponse> Validate(RoatpWithdrawApplicationViewModel viewModel);
    }
}



