using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Validators;

public class RoatpRemoveApplicationViewModelValidator : IRoatpRemoveApplicationViewModelValidator
{
    private const string NoSelectionErrorMessage = "Select if you are sure you want to remove this application";
    private const string ErrorEnterComments = "Enter internal comments";
    private const string TooManyWords = "Your internal comments must be 150 words or less";
    private const string ErrorEnterExternalComments = "Enter external comments";
    private const string TooManyWordsExternal = "Your external comments must be 500 words or less";

    public async Task<ValidationResponse> Validate(RoatpRemoveApplicationViewModel viewModel)
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

            if (string.IsNullOrEmpty(viewModel.OptionYesTextExternal))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionYesTextExternal", ErrorEnterExternalComments));
            }
            else
            {
                var wordCount = viewModel.OptionYesTextExternal.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount > 500)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionYesTextExternal", TooManyWordsExternal));
                }
            }
        }

        return await Task.FromResult(validationResponse);
    }
}

public interface IRoatpRemoveApplicationViewModelValidator
{
    Task<ValidationResponse> Validate(RoatpRemoveApplicationViewModel viewModel);
}



